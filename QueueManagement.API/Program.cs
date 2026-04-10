using QueueManagement.Application;
using QueueManagement.Infrastructure;
using QueueManagement.Application.Middleware;
using QueueManagement.Infrastructure.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using QueueManagement.API.Middlewares;
using Microsoft.Extensions.Configuration;
using QueueManagement.Domain.Entities.DTOs;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using Serilog;

// 🔥 QUAN TRỌNG: Tắt file watcher trong production
if (!System.Diagnostics.Debugger.IsAttached)
{
    Environment.SetEnvironmentVariable("DOTNET_USE_POLLING_FILE_WATCHER", "false");
    Environment.SetEnvironmentVariable("DOTNET_WATCH", "false");
}

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
    ContentRootPath = Directory.GetCurrentDirectory()
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// ===== 1. CẤU HÌNH SERILOG =====
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration)
          .Enrich.WithProperty("Application", "QueueManagement")
          .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);
    config.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
});

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// ===== 2. DIAGNOSTIC CONTEXT =====
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<Serilog.Core.LoggingLevelSwitch>();

// ===== 3. CẤU HÌNH CONFIGURATION =====
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false);
builder.Configuration.AddEnvironmentVariables();

// ===== 4. INFRASTRUCTURE & SERVICES =====
#region Infrastructure & Service
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplication();
#endregion

// ===== 5. AUTHENTICATION =====
#region Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"];

builder.Services.Configure<JwtSettings>(jwtSettings);
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero,
        RequireExpirationTime = true,
        RequireSignedTokens = true
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new { error = "You are not authorized", statusCode = 401 });
            return context.Response.WriteAsync(result);
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("AdminOrUser", policy => policy.RequireRole("Admin", "User"));
});
#endregion

// ===== 6. CONTROLLERS =====
#region Controller
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
#endregion

// ===== 7. SWAGGER =====
#region API EXPLORER & SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QueueManagement API",
        Version = "v1",
        Description = "API quản lý hàng đợi"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] {} }
    });
});

builder.Services.AddApiVersioning(option =>
{
    option.DefaultApiVersion = new ApiVersion(1, 0);
    option.AssumeDefaultVersionWhenUnspecified = true;
    option.ReportApiVersions = true;
    option.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
#endregion

// ===== 8. CORS =====
#region CORS
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

if (allowedOrigins == null || allowedOrigins.Length == 0)
{
    Console.WriteLine("WARNING: No allowed origins configured. Using defaults.");
    allowedOrigins = new[]
    {
        "https://queue-management-ui.vercel.app",
        "https://www.queue-management-ui.vercel.app",
        "http://localhost:5173",
        "https://localhost:5173",
        "http://localhost:3000",
        "https://localhost:3000",
        "http://localhost:5000",
        "https://localhost:7164"
    };
}

Console.WriteLine("🔧 Allowed Origins:");
foreach (var origin in allowedOrigins)
{
    Console.WriteLine($"  - {origin}");
}

builder.Services.AddCors(option =>
{
    option.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
});
#endregion

// ===== 9. SIGNALR =====
#region REAL-TIME & OTHERS
builder.Services.AddSignalR();
#endregion

var app = builder.Build();

// ==============================================
// ===== MIDDLEWARE PIPELINE - ĐÚNG THỨ TỰ =====
// ==============================================

// ✅ 1. CORS middleware (PHẢI ĐẦU TIÊN)
// Lý do: Xử lý preflight requests (OPTIONS) trước khi request đi vào pipeline
app.UseCors("FrontendPolicy");

// ✅ 2. Exception handling (BẮT LỖI TOÀN BỘ)
// Lý do: Phải bắt lỗi từ tất cả các middleware phía sau
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

// ✅ 3. HTTPS Redirection
// Lý do: Chuyển HTTP sang HTTPS trước khi xử lý
app.UseHttpsRedirection();

// ✅ 4. Routing
// Lý do: Xác định route cho request
app.UseRouting();

// ✅ 5. API Versioning
// Lý do: Xác định version API trước khi authentication
app.UseApiVersioning();

// ✅ 6. Authentication (PHẢI TRƯỚC Authorization)
// Lý do: Xác định danh tính người dùng trước khi kiểm tra quyền
app.UseAuthentication();

// ✅ 7. Authorization
// Lý do: Kiểm tra quyền sau khi đã xác định danh tính
app.UseAuthorization();

// ✅ 8. Swagger (Chỉ Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "QueueManagement API V1");
        c.RoutePrefix = "swagger";
    });
}

app.Logger.LogInformation("🚀 URLs: {urls}", app.Urls);

// ✅ 9. Endpoints (LUÔN CUỐI CÙNG)
// Lý do: Xử lý request sau khi đã qua tất cả middleware
app.MapControllers();
app.MapHub<QueueHub>("/queue-hub");

// ===== LOG KHI APP START =====
Log.Information("🚀 Application started successfully");
Log.Information("🌍 Environment: {Environment}", app.Environment.EnvironmentName);
Log.Information("🔧 CORS Allowed Origins: {origins}", string.Join(", ", allowedOrigins));

app.Run();

Log.CloseAndFlush();
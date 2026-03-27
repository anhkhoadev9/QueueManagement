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
using Serilog; // Using Serilog

var builder = WebApplication.CreateBuilder(args);

// ===== 1. CẤU HÌNH SERILOG - CHỈ 1 LẦN =====
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration)
          .Enrich.WithProperty("Application", "QueueManagement")
          .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);

    // Log ra console để dễ debug
    config.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

    // Log ra Seq nếu có cấu hình
    var seqServerUrl = context.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341";
    config.WriteTo.Seq(seqServerUrl);
});

// ===== 2. THÊM DIAGNOSTIC CONTEXT CHO SERILOG =====
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<Serilog.Core.LoggingLevelSwitch>();

// ===== 3. CÁC CẤU HÌNH KHÁC =====
#region Infratruture & Service
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplication();
#endregion

#region Authentication
// Đọc cấu hình JWT từ appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"];

// Đăng ký JwtSettings vào DI container
builder.Services.Configure<JwtSettings>(jwtSettings);
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
// Cấu hình Authentication
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

            var result = JsonSerializer.Serialize(new
            {
                error = "You are not authorized",
                statusCode = 401
            });

            return context.Response.WriteAsync(result);
        }
    };
});

// Thêm Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("AdminOrUser", policy => policy.RequireRole("Admin", "User"));
});
#endregion

#region Controller
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
#endregion

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

    // Thêm cấu hình JWT cho Swagger
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
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
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

#region CORS
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

if (allowedOrigins == null || allowedOrigins.Length == 0)
{
    Console.WriteLine("WARNING: No allowed origins configured. Using defaults.");
    allowedOrigins = new[]
    {
        "http://localhost:3000",
        "https://localhost:3000",
        "http://localhost:5000",
        "https://localhost:7164"
    };
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

#region REAL-TIME & OTHERS
builder.Services.AddSignalR();
#endregion

var app = builder.Build();

// ===== 4. MIDDLEWARE PIPELINE =====
// 1. Exception Handler - Đầu tiên
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

// 2. HTTPS Redirection
app.UseHttpsRedirection();

// 3. CORS - Trước mọi thứ khác
app.UseCors("FrontendPolicy");

// 4. Routing
app.UseRouting();

// 5. API Versioning
app.UseApiVersioning();

// 6. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 7. Swagger - Chỉ Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "QueueManagement API V1");
        c.RoutePrefix = "swagger";
    });
}

// 8. Endpoints - Cuối cùng
app.MapControllers();
app.MapHub<QueueHub>("/queue-hub");

// ===== 5. TEST LOG KHI APP START =====
Log.Information("🚀 Application started successfully");
Log.Information("📊 Seq URL: http://localhost:5341");
Log.Information("🌍 Environment: {Environment}", app.Environment.EnvironmentName);

app.Run();

// ===== 6. DỌN DẸP KHI APP TẮT =====
Log.CloseAndFlush();
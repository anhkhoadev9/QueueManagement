using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QueueManagement.Domain.Interfaces;
using QueueManagement.Infrastructure.Persistence;
using QueueManagement.Infrastructure.Persistence.Context;
using QueueManagement.Infrastructure.Persistence.Repositories;
using QueueManagement.Application.Common.Interfaces;
using QueueManagement.Infrastructure.SignalR;
using Microsoft.AspNetCore.Identity;
using QueueManagement.Infrastructure.Identity;
using QueueManagement.Infrastructure.Persistence.Authentication;

namespace QueueManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<QueueManagementDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            //Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
              .AddEntityFrameworkStores<QueueManagementDbContext>()
              .AddDefaultTokenProviders();
            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositories (for direct injection if needed)
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IQueueTicketRepository, QueueTicketRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<ITicketHistoryRepository, TicketHistoryRepository>();
            services.AddScoped<IFeedbackRepository, FeedBackRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IEmailLogRepository, EmailLogRepository>();
            //Auth
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            //Services
            services.AddScoped<IPasswordGenerator, PasswordGenerator>();
            // SignalR
            services.AddScoped<IQueueHubService, SignalRService>();



            return services;
        }

    }
}

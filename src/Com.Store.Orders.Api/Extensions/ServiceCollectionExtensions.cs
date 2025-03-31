using Com.Store.Orders.Api.Authentication;
using Com.Store.Orders.Api.Constants;
using Com.Store.Orders.Domain.Data.Enums;
using Com.Store.Orders.Domain.Data.Extensions;
using Com.Store.Orders.Domain.Data.Options;
using Com.Store.Orders.Domain.Services.Extensions;
using Com.Store.Orders.Domain.Services.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Com.Store.Orders.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions(configuration);
            services.AddDataLayer(configuration);
            services.AddDomainLayer(configuration);
            services.AddCache(configuration);
            services.AddAuthentication(configuration);
            services.AddAuthorization(configuration);
            return services;
        }

        private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConnectionStringsOptions>(options =>
            {
                configuration.GetSection(ConnectionStringsOptions.SectionName).Bind(options);
            });
            services.Configure<AwsMessagingOptions>(options =>
            {
                configuration.GetSection(AwsMessagingOptions.SectionName).Bind(options);
            });
            services.Configure<CachingOptions>(options =>
            {
                configuration.GetSection(CachingOptions.SectionName).Bind(options);
            });
            services.Configure<JwtSettings>(options =>
            {
                configuration.GetSection(JwtSettings.SectionName).Bind(options);
            });
            return services;
        }

        private static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
        {
            var cachingOptions = configuration.GetSection(CachingOptions.SectionName).Get<CachingOptions>();
            var connectionStringsOptions = configuration.GetSection(ConnectionStringsOptions.SectionName).Get<ConnectionStringsOptions>();
            services.AddOutputCache();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionStringsOptions.RedisConnectionString;
                options.InstanceName = cachingOptions.Prefix;
            });
            services.AddOutputCache(options =>
            {
                options.AddBasePolicy(builder =>
                {
                    builder.Expire(TimeSpan.FromMinutes(cachingOptions.ExpirationInMinutes));
                });
            });

            return services;
        }

        private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey!))
                    };
                });

            return services;
        }

        private static IServiceCollection AddAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationPolicyNames.RequireAdminOrConfirmingManagerRole, policy =>
                    policy.RequireRole(UserRole.Admin.ToString(), UserRole.ConfirmingManager.ToString()));
                options.AddPolicy(AuthorizationPolicyNames.RequireAdminOrShippingManagerRole, policy =>
                    policy.RequireRole(UserRole.Admin.ToString(), UserRole.ShippingManager.ToString()));
            });

            return services;
        }
    }
}

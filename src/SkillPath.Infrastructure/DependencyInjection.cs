using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillPath.Application.Interfaces;
using SkillPath.Application.Services;
using SkillPath.Domain.Repositories;
using SkillPath.Infrastructure.Data;
using SkillPath.Infrastructure.Repositories;
using SkillPath.Infrastructure.Security;

namespace SkillPath.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string jwtKey)
    {
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("Default");
            var provider = configuration.GetValue<string>("Database:Provider");
            var databaseName = configuration.GetValue<string>("Database:InMemoryName") ?? "SkillPathDb";

            if (string.Equals(provider, "InMemory", StringComparison.OrdinalIgnoreCase))
            {
                options.UseInMemoryDatabase(databaseName);
            }
            else if (!string.IsNullOrWhiteSpace(connectionString) &&
                     string.Equals(provider, "Oracle", StringComparison.OrdinalIgnoreCase))
            {
                options.UseOracle(connectionString, oracleOptions =>
                {
                    oracleOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                });
            }
            else if (!string.IsNullOrWhiteSpace(connectionString) &&
                     string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                });
            }
            else
            {
                // Fallback to InMemory if no provider is specified
                options.UseInMemoryDatabase(databaseName);
            }
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStaticCatalogRepository, StaticCatalogRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();

        services.AddScoped<ITokenProvider>(_ => new JwtTokenProvider(jwtKey));
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPlanService, PlanService>();
        services.AddScoped<IPlanServiceV2, PlanServiceV2>();
        services.AddScoped<ISkillService, SkillService>();
        services.AddScoped<ICourseService, CourseService>();

        return services;
    }
}

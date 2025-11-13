using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillShiftHub.Application.Interfaces;
using SkillShiftHub.Application.Services;
using SkillShiftHub.Domain.Repositories;
using SkillShiftHub.Infrastructure.Data;
using SkillShiftHub.Infrastructure.Repositories;
using SkillShiftHub.Infrastructure.Security;

namespace SkillShiftHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string jwtKey)
    {
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("Default");
            var provider = configuration.GetValue<string>("Database:Provider");

            if (!string.IsNullOrWhiteSpace(connectionString) &&
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
                options.UseInMemoryDatabase("SkillShiftHubDb");
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

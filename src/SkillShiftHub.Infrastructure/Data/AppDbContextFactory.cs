using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SkillShiftHub.Infrastructure.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Design-time connection string; replace by configuring appsettings for runtime.
        var connectionString = Environment.GetEnvironmentVariable("SKILLSHIFTHUB_MIGRATIONS_CONNECTION")
                               ?? "User Id=skillshift;Password=skillshift;Data Source=//localhost:1521/XEPDB1;";

        optionsBuilder.UseOracle(connectionString, options =>
        {
            options.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
        });

        return new AppDbContext(optionsBuilder.Options);
    }
}


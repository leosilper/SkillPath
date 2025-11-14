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
        // You can override this by setting the SKILLSHIFTHUB_MIGRATIONS_CONNECTION environment variable
        var connectionString = Environment.GetEnvironmentVariable("SKILLSHIFTHUB_MIGRATIONS_CONNECTION")
                               ?? "User Id=rm557598;Password=040903;Data Source=oracle.fiap.com.br:1521/ORCL";

        optionsBuilder.UseOracle(connectionString, options =>
        {
            options.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
        });

        return new AppDbContext(optionsBuilder.Options);
    }
}


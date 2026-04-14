using BuildingBlocks.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Scaffold.Weather.Infrastructure.Persistence;

public sealed class WeatherDbContextFactory : IDesignTimeDbContextFactory<WeatherDbContext>
{
    public WeatherDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WeatherDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString: "Host=_design-time_;Database=_design-time_",
            npgsqlOptionsAction: o => o.MigrationsHistoryTable("__EFMigrationsHistory", typeof(WeatherDbContext).ToDbContextSchemaName())
        );

        return new WeatherDbContext(optionsBuilder.Options);
    }
}

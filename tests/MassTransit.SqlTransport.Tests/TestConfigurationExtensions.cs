namespace MassTransit.DbTransport.Tests;

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SqlTransport.PostgreSql;


public static class TestConfigurationExtensions
{
    public static IServiceCollection ConfigurePostgresTransport(this IServiceCollection services, bool create = true, bool delete = false)
    {
        services.AddOptions<SqlTransportOptions>().Configure(options =>
        {
            options.Host = "localhost";
            options.Database = "masstransit_transport_tests";
            options.Schema = "transport";
            options.Role = "transport";
            options.Username = "unit_tests";
            options.Password = "H4rd2Gu3ss!";
            options.AdminUsername = "postgres";
            options.AdminPassword = "Password12!";
        });

        services.AddPostgresMigrationHostedService(create, delete);

        return services;
    }

    public static PostgresSqlTransportConnection GetTransportConnection(this IServiceProvider provider)
    {
        return PostgresSqlTransportConnection.GetDatabaseConnection(provider.GetRequiredService<IOptions<SqlTransportOptions>>().Value);
    }
}

namespace MassTransit
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using SqlTransport;
    using SqlTransport.PostgreSql;


    public static class PostgresSqlTransportConfigurationExtensions
    {
        public static IServiceCollection AddPostgresMigrationHostedService(this IServiceCollection services, bool create = true, bool delete = false)
        {
            services.AddPostgresMigrationHostedService(options =>
            {
                options.CreateDatabase = create;
                options.DeleteDatabase = delete;
                options.CreateInfrastructure = create;
            });

            return services;
        }

        public static IServiceCollection AddPostgresMigrationHostedService(this IServiceCollection services, Action<SqlTransportMigrationOptions> options)
        {
            services.AddTransient<ISqlTransportDatabaseMigrator, PostgresDatabaseMigrator>();

            services.AddOptions<SqlTransportOptions>();
            services.AddOptions<SqlTransportMigrationOptions>()
                .Configure(options);
            services.AddHostedService<SqlTransportMigrationHostedService>();

            return services;
        }
    }
}

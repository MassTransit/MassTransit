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
                options.CreateSchema = create;
                options.CreateInfrastructure = create;
                options.DeleteDatabase = delete;
            });

            return services;
        }

        public static IServiceCollection AddPostgresMigrationHostedService(this IServiceCollection services, Action<SqlTransportMigrationOptions>? configure)
        {
            services.AddTransient<ISqlTransportDatabaseMigrator, PostgresDatabaseMigrator>();

            services.AddOptions<SqlTransportOptions>();
            services.AddOptions<SqlTransportMigrationOptions>()
                .Configure(options =>
                {
                    options.CreateDatabase = true;
                    options.CreateSchema = true;
                    options.CreateInfrastructure = true;
                    options.DeleteDatabase = false;

                    configure?.Invoke(options);
                });
            services.AddHostedService<SqlTransportMigrationHostedService>();

            return services;
        }
    }
}

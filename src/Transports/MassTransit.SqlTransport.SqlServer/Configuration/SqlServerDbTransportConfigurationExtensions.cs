namespace MassTransit
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using SqlTransport;
    using SqlTransport.SqlServer;


    public static class SqlServerDbTransportConfigurationExtensions
    {
        public static IServiceCollection AddSqlServerMigrationHostedService(this IServiceCollection services, bool create = true, bool delete = false)
        {
            services.AddSqlServerMigrationHostedService(options =>
            {
                options.CreateDatabase = create;
                options.CreateSchema = create;
                options.CreateInfrastructure = create;
                options.DeleteDatabase = delete;
            });

            return services;
        }

        public static IServiceCollection AddSqlServerMigrationHostedService(this IServiceCollection services, Action<SqlTransportMigrationOptions>? configure)
        {
            services.AddTransient<ISqlTransportDatabaseMigrator, SqlServerDatabaseMigrator>();

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

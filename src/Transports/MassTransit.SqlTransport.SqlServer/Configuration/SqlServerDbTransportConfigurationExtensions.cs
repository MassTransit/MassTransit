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
                options.DeleteDatabase = delete;
                options.CreateInfrastructure = create;
            });

            return services;
        }

        public static IServiceCollection AddSqlServerMigrationHostedService(this IServiceCollection services, Action<SqlTransportMigrationOptions> options)
        {
            services.AddTransient<ISqlTransportDatabaseMigrator, SqlServerDatabaseMigrator>();

            services.AddOptions<SqlTransportOptions>();
            services.AddOptions<SqlTransportMigrationOptions>()
                .Configure(options);
            services.AddHostedService<SqlTransportMigrationHostedService>();

            return services;
        }
    }
}

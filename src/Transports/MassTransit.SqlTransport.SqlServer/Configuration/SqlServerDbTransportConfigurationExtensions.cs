namespace MassTransit
{
    using Microsoft.Extensions.DependencyInjection;
    using SqlTransport;
    using SqlTransport.SqlServer;


    public static class SqlServerDbTransportConfigurationExtensions
    {
        public static IServiceCollection AddSqlServerMigrationHostedService(this IServiceCollection services, bool create = true, bool delete = false)
        {
            services.AddTransient<ISqlTransportDatabaseMigrator, SqlServerDatabaseMigrator>();

            services.AddOptions<SqlTransportOptions>();
            services.AddOptions<SqlTransportMigrationOptions>()
                .Configure(options =>
                {
                    options.CreateDatabase = create;
                    options.DeleteDatabase = delete;
                });
            services.AddHostedService<SqlTransportMigrationHostedService>();

            return services;
        }
    }
}

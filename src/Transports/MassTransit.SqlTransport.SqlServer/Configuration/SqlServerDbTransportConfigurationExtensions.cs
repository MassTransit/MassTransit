namespace MassTransit
{
    using Microsoft.Extensions.DependencyInjection;
    using SqlTransport;
    using SqlTransport.SqlServer;


    public static class SqlServerDbTransportConfigurationExtensions
    {
        public static IServiceCollection AddSqlServerMigrationHostedService(this IServiceCollection services, bool createDatabase = true, bool delete = false, bool createInfrastructure = true)
        {
            services.AddTransient<ISqlTransportDatabaseMigrator, SqlServerDatabaseMigrator>();

            services.AddOptions<SqlTransportOptions>();
            services.AddOptions<SqlTransportMigrationOptions>()
                .Configure(options =>
                {
                    options.CreateDatabase = createDatabase;
                    options.CreateInfrastructure = createInfrastructure;
                    options.DeleteDatabase = delete;
                });
            services.AddHostedService<SqlTransportMigrationHostedService>();

            return services;
        }
    }
}

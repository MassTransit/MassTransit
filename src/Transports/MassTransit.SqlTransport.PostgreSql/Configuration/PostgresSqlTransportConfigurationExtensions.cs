namespace MassTransit
{
    using Microsoft.Extensions.DependencyInjection;
    using SqlTransport;
    using SqlTransport.PostgreSql;


    public static class PostgresSqlTransportConfigurationExtensions
    {
        public static IServiceCollection AddPostgresMigrationHostedService(this IServiceCollection services, bool createDatabase = true, bool delete = false, bool createInfrastructure = true)
        {
            services.AddTransient<ISqlTransportDatabaseMigrator, PostgresDatabaseMigrator>();

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

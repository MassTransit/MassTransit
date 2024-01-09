namespace MassTransit
{
    using Microsoft.Extensions.DependencyInjection;
    using SqlTransport;
    using SqlTransport.PostgreSql;


    public static class PostgresSqlTransportConfigurationExtensions
    {
        public static IServiceCollection AddPostgresMigrationHostedService(this IServiceCollection services, bool create = true, bool delete = false)
        {
            services.AddTransient<ISqlTransportDatabaseMigrator, PostgresDatabaseMigrator>();

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

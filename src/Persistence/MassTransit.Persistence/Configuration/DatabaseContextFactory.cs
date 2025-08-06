namespace MassTransit.Persistence.Configuration
{
    using Integration.Saga;


    public delegate Task<DatabaseContext<TSaga>> DatabaseContextFactory<TSaga>(IServiceProvider serviceProvider)
        where TSaga : class;
}

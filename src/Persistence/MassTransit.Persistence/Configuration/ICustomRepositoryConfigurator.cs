namespace MassTransit.Persistence.Configuration
{
    /// <summary>
    /// Enables an ADO.NET-based saga repository for precise control over the persistence process.
    /// </summary>
    public interface ICustomRepositoryConfigurator<TSaga>
        where TSaga : class
    {
        /// <summary>
        /// Allows for full control over creating the saga repository.  Only use this if the
        /// other configuration methods are insufficiently flexible to configure the repository.
        /// </summary>
        ICustomRepositoryConfigurator<TSaga> SetContextFactory(DatabaseContextFactory<TSaga> contextFactory);
    }
}

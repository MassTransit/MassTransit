namespace MassTransit
{
    using Microsoft.Azure.Cosmos;


    /// <summary>
    /// An interface of a factory for creating Cosmos DB clients
    /// </summary>
    public interface ICosmosClientFactory
    {
        /// <summary>
        /// Gets a <see cref="CosmosClient" /> used to interact with a CosmosDB database account
        /// </summary>
        /// <param name="clientName">The name of the client</param>
        /// <returns>A <see cref="CosmosClient" /> used to interact with Cosmos DB</returns>
        CosmosClient GetCosmosClient<T>(string clientName)
            where T : class, ISaga;
    }
}

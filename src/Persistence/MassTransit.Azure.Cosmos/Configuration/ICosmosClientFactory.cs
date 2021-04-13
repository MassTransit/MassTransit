namespace MassTransit.Azure.Cosmos
{
    using Microsoft.Azure.Cosmos;
    using Newtonsoft.Json;

    /// <summary>
    /// An interface of a factory for creating Cosmos DB clients
    /// </summary>
    public interface ICosmosClientFactory
    {
        /// <summary>
        /// Gets a <see cref="CosmosClient"/> used to interact with a CosmosDB database account
        /// </summary>
        /// <param name="cosmosClientName">The name of the client</param>
        /// <param name="serializerSettings">Serializer settings for the client</param>
        /// <returns>A <see cref="CosmosClient"/> used to interact with Cosmos DB</returns>
        CosmosClient GetCosmosClient(string cosmosClientName, JsonSerializerSettings serializerSettings);
    }
}

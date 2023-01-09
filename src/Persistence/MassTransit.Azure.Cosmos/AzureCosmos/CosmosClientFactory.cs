namespace MassTransit.AzureCosmos
{
    using Azure.Identity;
    using Microsoft.Azure.Cosmos;


    static class CosmosClientFactory
    {
        public static CosmosClient CreateClient(CosmosAuthSettings authSettings, CosmosClientOptions clientOptions)
        {
            if (authSettings.TokenCredential != null)
                return new CosmosClient(authSettings.AccountEndpoint, authSettings.TokenCredential, clientOptions);

            if (authSettings.ConnectionString != null)
                return new CosmosClient(authSettings.ConnectionString, clientOptions);

            if (authSettings.AuthKeyOrResourceToken != null)
                return new CosmosClient(authSettings.AccountEndpoint, authSettings.AuthKeyOrResourceToken, clientOptions);

            return new CosmosClient(authSettings.AccountEndpoint, new DefaultAzureCredential(), clientOptions);
        }
    }
}

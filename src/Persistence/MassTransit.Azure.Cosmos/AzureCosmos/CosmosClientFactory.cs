using Azure.Identity;
using Microsoft.Azure.Cosmos;

namespace MassTransit.AzureCosmos
{
    internal static class CosmosClientFactory
    {
        public static CosmosClient CreateClient(CosmosAuthSettings authSettings, CosmosClientOptions clientOptions)
        {
            if (authSettings.TokenCredential != null)
            {
                return new CosmosClient(authSettings.AccountEndpoint, authSettings.TokenCredential, clientOptions);
            }
            else if (authSettings.ConnectionString != null)
            {
                return new CosmosClient(authSettings.ConnectionString, clientOptions);
            }
            else if (authSettings.AuthKeyOrResourceToken != null)
            {
                return new CosmosClient(authSettings.AccountEndpoint, authSettings.AuthKeyOrResourceToken, clientOptions);
            }
            else
            {
                return new CosmosClient(authSettings.AccountEndpoint, new DefaultAzureCredential(), clientOptions);
            }
        }
    }
}

namespace MassTransit.AzureCosmos
{
    using Azure.Core;


    public class CosmosAuthSettings
    {
        public CosmosAuthSettings()
        {
        }

        public CosmosAuthSettings(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public CosmosAuthSettings(string accountEndpoint, string authKeyOrResourceToken)
        {
            AccountEndpoint = accountEndpoint;
            AuthKeyOrResourceToken = authKeyOrResourceToken;
        }

        public CosmosAuthSettings(string accountEndpoint, TokenCredential tokenCredential)
        {
            AccountEndpoint = accountEndpoint;
            TokenCredential = tokenCredential;
        }

        /// <summary>
        /// The account endpoint to connect to Cosmos DB
        /// </summary>
        public string AccountEndpoint { get; set; }

        /// <summary>
        /// The cosmos account key or resource token to use to connect to Cosmos DB
        /// </summary>
        /// <remarks>
        /// This property cannot be used if <see cref="TokenCredential" /> is being used.
        /// </remarks>
        public string AuthKeyOrResourceToken { get; set; }

        /// <summary>
        /// The connection string to use to connect to Cosmos DB
        /// </summary>
        /// <remarks>
        /// This property cannot be used if <see cref="TokenCredential" /> or
        /// <see cref="AuthKeyOrResourceToken" /> is being used.
        /// </remarks>
        public string ConnectionString { get; set; }

        /// <summary>
        /// The token credential to use to connect to Cosmos DB
        /// </summary>
        public TokenCredential TokenCredential { get; set; }
    }
}

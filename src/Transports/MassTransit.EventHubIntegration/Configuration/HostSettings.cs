namespace MassTransit.EventHubIntegration.Configuration
{
    using Azure.Core;


    public class HostSettings :
        IHostSettings
    {
        public HostSettings(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public HostSettings(string fullyQualifiedNamespace, TokenCredential tokenCredential)
        {
            FullyQualifiedNamespace = fullyQualifiedNamespace;
            TokenCredential = tokenCredential;
        }

        public string ConnectionString { get; }
        public string FullyQualifiedNamespace { get; }
        public TokenCredential TokenCredential { get; }
    }
}

namespace MassTransit.EventHubIntegration.Configuration
{
    using Azure.Core;


    public class HostSettings :
        IHostSettings
    {
        public string ConnectionString { get; set; }
        public string FullyQualifiedNamespace { get; set; }
        public TokenCredential TokenCredential { get; set; }
    }
}

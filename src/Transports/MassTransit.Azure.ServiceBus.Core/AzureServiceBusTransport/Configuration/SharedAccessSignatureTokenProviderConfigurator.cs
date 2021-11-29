namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using Azure;


    public class SharedAccessSignatureTokenProviderConfigurator :
        ISharedAccessSignatureTokenProviderConfigurator
    {
        public AzureSasCredential SasCredential { get; set; }
    }
}

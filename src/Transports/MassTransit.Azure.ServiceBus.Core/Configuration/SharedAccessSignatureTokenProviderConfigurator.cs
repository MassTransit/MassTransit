namespace MassTransit.Azure.ServiceBus.Core
{
    using global::Azure;


    public class SharedAccessSignatureTokenProviderConfigurator :
        ISharedAccessSignatureTokenProviderConfigurator
    {
        public AzureSasCredential SasCredential { get; set; }
    }
}

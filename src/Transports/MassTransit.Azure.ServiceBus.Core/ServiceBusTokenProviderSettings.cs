namespace MassTransit.Azure.ServiceBus.Core
{
    using global::Azure;
    using global::Azure.Core;

    public interface ServiceBusTokenProviderSettings
    {
        AzureNamedKeyCredential NamedKeyCredential { get; }
        AzureSasCredential SasCredential { get; }
        TokenCredential TokenCredential { get; }
    }
}

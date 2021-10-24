namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using global::Azure;


    public interface ISharedAccessSignatureTokenProviderConfigurator
    {
        AzureSasCredential SasCredential { set; }
    }
}

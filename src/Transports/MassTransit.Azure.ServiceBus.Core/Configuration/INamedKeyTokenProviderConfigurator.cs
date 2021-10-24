namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using global::Azure;

    public interface INamedKeyTokenProviderConfigurator
    {
        AzureNamedKeyCredential NamedKeyCredential { set; }
    }
}

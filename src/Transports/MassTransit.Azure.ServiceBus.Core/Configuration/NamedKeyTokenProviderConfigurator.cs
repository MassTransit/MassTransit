namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using global::Azure;


    public class NamedKeyTokenProviderConfigurator :
        INamedKeyTokenProviderConfigurator
    {
        public AzureNamedKeyCredential NamedKeyCredential { get; set; }
    }
}

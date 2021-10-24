namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using global::Azure;
    using global::Azure.Core;


    public class BasicAzureServiceBusAccountSettings :
        ServiceBusTokenProviderSettings
    {
        static readonly string KeyName = Configuration.KeyName;
        static readonly string SharedAccessKey = Configuration.SharedAccessKey;

        public BasicAzureServiceBusAccountSettings()
        {
        }

        public AzureNamedKeyCredential NamedKeyCredential => new AzureNamedKeyCredential(KeyName, SharedAccessKey);
        public AzureSasCredential SasCredential { get; }
        public TokenCredential TokenCredential { get; }
    }
}

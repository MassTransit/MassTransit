namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using global::Azure;
    using global::Azure.Core;


    public class TestAzureServiceBusAccountSettings :
        ServiceBusTokenProviderSettings
    {
        public TestAzureServiceBusAccountSettings()
        {
        }

        public AzureNamedKeyCredential NamedKeyCredential => new AzureNamedKeyCredential(Configuration.KeyName, Configuration.SharedAccessKey);
        public AzureSasCredential SasCredential { get; }
        public TokenCredential TokenCredential { get; }
    }
}

namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using Microsoft.Azure.ServiceBus.Primitives;


    public class TestAzureServiceBusAccountSettings :
        ServiceBusTokenProviderSettings
    {
        public TestAzureServiceBusAccountSettings()
        {
            TokenTimeToLive = TimeSpan.FromDays(1);
            TokenScope = TokenScope.Namespace;
        }

        public string KeyName => Configuration.KeyName;

        public string SharedAccessKey => Configuration.SharedAccessKey;

        public TimeSpan TokenTimeToLive { get; }

        public TokenScope TokenScope { get; }
    }
}

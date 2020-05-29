namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using Microsoft.Azure.ServiceBus.Primitives;


    public class BasicAzureServiceBusAccountSettings :
        ServiceBusTokenProviderSettings
    {
        static readonly string KeyName = Configuration.KeyName;
        static readonly string SharedAccessKey = Configuration.SharedAccessKey;
        readonly TokenScope _tokenScope;
        readonly TimeSpan _tokenTimeToLive;

        public BasicAzureServiceBusAccountSettings()
        {
            _tokenTimeToLive = TimeSpan.FromDays(1);
            _tokenScope = TokenScope.Namespace;
        }

        string ServiceBusTokenProviderSettings.KeyName => KeyName;

        string ServiceBusTokenProviderSettings.SharedAccessKey => SharedAccessKey;

        TimeSpan ServiceBusTokenProviderSettings.TokenTimeToLive => _tokenTimeToLive;

        TokenScope ServiceBusTokenProviderSettings.TokenScope => _tokenScope;
    }
}

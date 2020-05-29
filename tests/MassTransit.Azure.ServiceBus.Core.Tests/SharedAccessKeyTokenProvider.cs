namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using Microsoft.Azure.ServiceBus.Primitives;


    public class SharedAccessKeyTokenProvider :
        IAzureServiceBusTokenProvider
    {
        readonly ServiceBusTokenProviderSettings _settings;

        public SharedAccessKeyTokenProvider(ServiceBusTokenProviderSettings settings)
        {
            _settings = settings;
        }

        public ITokenProvider GetTokenProvider()
        {
            return TokenProvider.CreateSharedAccessSignatureTokenProvider(_settings.KeyName, _settings.SharedAccessKey,
                _settings.TokenTimeToLive, _settings.TokenScope);
        }
    }
}

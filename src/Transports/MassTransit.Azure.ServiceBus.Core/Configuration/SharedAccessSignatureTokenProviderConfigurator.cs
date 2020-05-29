namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using Microsoft.Azure.ServiceBus.Primitives;


    public class SharedAccessSignatureTokenProviderConfigurator :
        ISharedAccessSignatureTokenProviderConfigurator
    {
        public string KeyName { get; set; }
        public string SharedAccessKey { get; set; }
        public TimeSpan TokenTimeToLive { get; set; }
        public TokenScope TokenScope { get; set; }

        public TokenProvider GetTokenProvider()
        {
            return TokenProvider.CreateSharedAccessSignatureTokenProvider(KeyName, SharedAccessKey, TokenTimeToLive, TokenScope);
        }
    }
}

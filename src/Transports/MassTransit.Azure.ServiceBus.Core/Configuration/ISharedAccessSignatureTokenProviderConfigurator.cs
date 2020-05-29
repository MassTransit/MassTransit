namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using Microsoft.Azure.ServiceBus.Primitives;


    public interface ISharedAccessSignatureTokenProviderConfigurator :
        ITokenProviderConfigurator
    {
        string KeyName { set; }
        string SharedAccessKey { set; }
        TimeSpan TokenTimeToLive { set; }
        TokenScope TokenScope { set; }
    }
}

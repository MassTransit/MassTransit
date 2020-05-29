namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using Microsoft.Azure.ServiceBus.Primitives;


    public interface ServiceBusTokenProviderSettings
    {
        string KeyName { get; }
        string SharedAccessKey { get; }
        TimeSpan TokenTimeToLive { get; }
        TokenScope TokenScope { get; }
    }
}

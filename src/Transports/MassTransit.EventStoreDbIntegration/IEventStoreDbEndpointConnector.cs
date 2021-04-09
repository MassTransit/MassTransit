using System;
using MassTransit.Registration;

namespace MassTransit.EventStoreDbIntegration
{
    public interface IEventStoreDbEndpointConnector
    {
        HostReceiveEndpointHandle ConnectEventStoreDbEndpoint(StreamCategory streamCategory, string subscriptionName,
            Action<IRiderRegistrationContext, IEventStoreDbReceiveEndpointConfigurator> configure);
    }
}

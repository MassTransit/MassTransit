namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using Azure.Messaging.EventHubs.Producer;
    using Transports;


    public class ConnectionContextSupervisor :
        TransportPipeContextSupervisor<ConnectionContext>,
        IConnectionContextSupervisor
    {
        public ConnectionContextSupervisor(IHostSettings hostSettings, IStorageSettings storageSettings, Action<EventHubProducerClientOptions> configureOptions)
            : base(new ConnectionContextFactory(hostSettings, storageSettings, configureOptions))
        {
        }
    }
}

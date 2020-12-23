namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System;
    using Contexts;
    using GreenPipes.Agents;
    using Transport;
    using Transports;


    public interface IConnectionContextSupervisor :
        ITransportSupervisor<ConnectionContext>,
        ISendTransportProvider,
        IPublishTransportProvider
    {
        IClientContextSupervisor CreateClientContextSupervisor(Func<IConnectionContextSupervisor, IPipeContextFactory<ClientContext>> factory);

        ISendEndpointContextSupervisor CreateSendEndpointContextSupervisor(SendSettings settings);
    }
}

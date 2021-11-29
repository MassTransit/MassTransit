namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading.Tasks;
    using Agents;
    using Transports;


    public interface IConnectionContextSupervisor :
        ITransportSupervisor<ConnectionContext>
    {
        IClientContextSupervisor CreateClientContextSupervisor(Func<IConnectionContextSupervisor, IPipeContextFactory<ClientContext>> factory);

        ISendEndpointContextSupervisor CreateSendEndpointContextSupervisor(SendSettings settings);

        Task<ISendTransport> CreateSendTransport(ReceiveEndpointContext context, Uri address);

        Task<ISendTransport> CreatePublishTransport<T>(ReceiveEndpointContext context, Uri publishAddress)
            where T : class;

        Uri NormalizeAddress(Uri address);
    }
}

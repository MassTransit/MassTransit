namespace MassTransit.InMemoryTransport
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Fabric;
    using Transports;


    public interface IInMemoryTransportProvider :
        IAgent,
        IProbeSite
    {
        IMessageFabric MessageFabric { get; }

        IInMemoryConsumeTopologyBuilder CreateConsumeTopologyBuilder();

        Task<ISendTransport> CreateSendTransport(ReceiveEndpointContext context, Uri address);

        Task<ISendTransport> CreatePublishTransport<T>(ReceiveEndpointContext context, Uri publishAddress)
            where T : class;

        Uri NormalizeAddress(Uri address);
    }
}

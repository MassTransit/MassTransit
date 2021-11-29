namespace MassTransit.GrpcTransport
{
    using System;
    using System.Threading.Tasks;
    using Fabric;
    using Transports;


    public interface IGrpcTransportProvider :
        IAgent,
        IProbeSite
    {
        Uri HostAddress { get; }
        NodeContext HostNodeContext { get; }
        IMessageFabric MessageFabric { get; }

        IGrpcHostNode HostNode { get; }

        /// <summary>
        /// Await this task to ensure the server and clients have started up successfully
        /// </summary>
        Task StartupTask { get; }

        Task<ISendTransport> CreateSendTransport(ReceiveEndpointContext context, Uri address);

        Task<ISendTransport> CreatePublishTransport<T>(ReceiveEndpointContext context, Uri publishAddress)
            where T : class;

        Uri NormalizeAddress(Uri address);
    }
}

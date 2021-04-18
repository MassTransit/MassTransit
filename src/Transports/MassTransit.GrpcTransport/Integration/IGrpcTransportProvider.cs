namespace MassTransit.GrpcTransport.Integration
{
    using System;
    using System.Threading.Tasks;
    using Contexts;
    using Fabric;
    using GreenPipes;
    using GreenPipes.Agents;
    using Transports;


    public interface IGrpcTransportProvider :
        IAgent,
        IProbeSite
    {
        Uri HostAddress { get; }
        NodeContext HostNodeContext { get; }

        IMessageFabric MessageFabric { get; }

        /// <summary>
        /// Await this task to ensure the server and clients have started up successfully
        /// </summary>
        Task StartupTask { get; }

        Task<ISendTransport> GetSendTransport(GrpcReceiveEndpointContext context, Uri address);

        Task<ISendTransport> GetPublishTransport<T>(GrpcReceiveEndpointContext context, Uri publishAddress)
            where T : class;

        Uri NormalizeAddress(Uri address);
    }
}

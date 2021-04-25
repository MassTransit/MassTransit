namespace MassTransit.GrpcTransport.Integration
{
    using System;
    using System.Threading.Tasks;
    using Contexts;
    using Fabric;
    using GreenPipes;
    using GreenPipes.Agents;


    public interface IGrpcTransportProvider :
        ISendTransportProvider,
        IPublishTransportProvider,
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
    }
}

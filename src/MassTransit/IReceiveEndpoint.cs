namespace MassTransit
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Monitoring.Health;
    using Pipeline;
    using Transports;


    /// <summary>
    /// A service endpoint has a inbound transport that pushes messages to consumers
    /// </summary>
    public interface IReceiveEndpoint :
        ISendEndpointProvider,
        IPublishEndpointProvider,
        IConsumePipeConnector,
        IRequestPipeConnector,
        IReceiveObserverConnector,
        IReceiveEndpointObserverConnector,
        IConsumeObserverConnector,
        IConsumeMessageObserverConnector,
        IEndpointHealth,
        IProbeSite
    {
        Task<ReceiveEndpointReady> Started { get; }
    }
}

namespace MassTransit.Conductor.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;


    /// <summary>
    /// Maintains a service endpoint cache for each message type
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IServiceClientMessageCache<TMessage> :
        IServiceClientMessageCache
        where TMessage : class
    {
        Task<IRequestSendEndpoint<TMessage>> GetServiceSendEndpoint(ClientFactoryContext clientFactoryContext, TMessage message,
            ConsumeContext consumeContext = default, CancellationToken cancellationToken = default);
    }


    public interface IServiceClientMessageCache
    {
        Guid ClientId { get; }

        Type MessageType { get; }

        Task UnlinkClient(IPublishEndpointProvider publishEndpointProvider);
    }
}

namespace MassTransit.Pipeline.Filters.Outbox
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;


    public class InMemoryOutboxPublishEndpointProvider :
        IPublishEndpointProvider
    {
        readonly OutboxContext _outboxContext;
        readonly IPublishEndpointProvider _publishEndpointProvider;

        public InMemoryOutboxPublishEndpointProvider(OutboxContext outboxContext, IPublishEndpointProvider publishEndpointProvider)
        {
            _outboxContext = outboxContext;
            _publishEndpointProvider = publishEndpointProvider;
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishEndpointProvider.ConnectPublishObserver(observer);
        }

        public IPublishEndpoint CreatePublishEndpoint(Uri sourceAddress, ConsumeContext context = null)
        {
            var publishEndpoint = _publishEndpointProvider.CreatePublishEndpoint(sourceAddress, context);

            return new OutboxPublishEndpoint(_outboxContext, publishEndpoint);
        }

        public Task<ISendEndpoint> GetPublishSendEndpoint<T>(T message)
            where T : class
        {
            return _publishEndpointProvider.GetPublishSendEndpoint<T>(message);
        }
    }
}
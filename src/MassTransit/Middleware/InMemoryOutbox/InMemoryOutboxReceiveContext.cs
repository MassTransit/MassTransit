namespace MassTransit.Middleware.InMemoryOutbox
{
    using Context;


    public class InMemoryOutboxReceiveContext :
        ReceiveContextProxy
    {
        public InMemoryOutboxReceiveContext(OutboxContext outboxContext, ReceiveContext context)
            : base(context)
        {
            SendEndpointProvider = new InMemoryOutboxSendEndpointProvider(outboxContext, context.SendEndpointProvider);

            PublishEndpointProvider = new InMemoryOutboxPublishEndpointProvider(outboxContext, context.PublishEndpointProvider);
        }

        public override IPublishEndpointProvider PublishEndpointProvider { get; }

        public override ISendEndpointProvider SendEndpointProvider { get; }
    }
}

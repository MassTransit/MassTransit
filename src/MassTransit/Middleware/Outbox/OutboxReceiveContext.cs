namespace MassTransit.Middleware.Outbox
{
    using Context;


    public class OutboxReceiveContext :
        ReceiveContextProxy
    {
        public OutboxReceiveContext(OutboxSendContext outboxContext, ReceiveContext context)
            : base(context)
        {
            SendEndpointProvider = new OutboxSendEndpointProvider(outboxContext, context.SendEndpointProvider);
            PublishEndpointProvider = new OutboxPublishEndpointProvider(outboxContext, context.PublishEndpointProvider);
        }

        public override IPublishEndpointProvider PublishEndpointProvider { get; }
        public override ISendEndpointProvider SendEndpointProvider { get; }
    }
}

namespace MassTransit.Middleware.Outbox
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;


    public abstract class OutboxConsumeContextProxy<TMessage> :
        ConsumeContextProxy<TMessage>,
        OutboxConsumeContext<TMessage>
        where TMessage : class
    {
        readonly OutboxConsumeOptions _options;

        protected OutboxConsumeContextProxy(ConsumeContext<TMessage> context, OutboxConsumeOptions options)
            : base(context)
        {
            _options = options;

            CapturedContext = context;

            var outboxReceiveContext = new OutboxReceiveContext(this, context.ReceiveContext);

            ReceiveContext = outboxReceiveContext;
            PublishEndpointProvider = outboxReceiveContext.PublishEndpointProvider;

            // if (context.TryGetPayload(out MessageSchedulerContext schedulerContext))
            // {
            //     _outboxSchedulerContext = new OutboxMessageSchedulerContext(schedulerContext, _clearToSend.Task);
            //     context.AddOrUpdatePayload(() => _outboxSchedulerContext, _ => _outboxSchedulerContext);
            // }
        }

        public ConsumeContext CapturedContext { get; }

        protected Guid ConsumerId => _options.ConsumerId;

        public abstract bool ContinueProcessing { get; set; }
        public abstract bool IsMessageConsumed { get; }
        public abstract bool IsOutboxDelivered { get; }
        public abstract int ReceiveCount { get; }
        public abstract long? LastSequenceNumber { get; }

        public abstract Task SetConsumed();
        public abstract Task SetDelivered();

        public abstract Task<List<OutboxMessageContext>> LoadOutboxMessages();

        public abstract Task NotifyOutboxMessageDelivered(OutboxMessageContext message);

        public abstract Task RemoveOutboxMessages();

        public abstract Task AddSend<T>(SendContext<T> context)
            where T : class;
    }
}

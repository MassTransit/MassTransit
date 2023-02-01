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
        readonly IServiceProvider _provider;

        protected OutboxConsumeContextProxy(ConsumeContext<TMessage> context, OutboxConsumeOptions options, IServiceProvider provider)
            : base(context)
        {
            CapturedContext = context;
            Options = options;
            _provider = provider;

            var outboxReceiveContext = new OutboxReceiveContext(this, context.ReceiveContext);

            ReceiveContext = outboxReceiveContext;
            PublishEndpointProvider = outboxReceiveContext.PublishEndpointProvider;

            if (context.TryGetPayload(out MessageSchedulerContext schedulerContext))
            {
                context.AddOrUpdatePayload<MessageSchedulerContext>(
                    () => new ConsumeMessageSchedulerContext(this, schedulerContext.SchedulerFactory),
                    existing => new ConsumeMessageSchedulerContext(this, existing.SchedulerFactory));
            }
        }

        protected OutboxConsumeOptions Options { get; }

        protected Guid ConsumerId => Options.ConsumerId;

        public ConsumeContext CapturedContext { get; }

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

        public object GetService(Type serviceType)
        {
            return _provider.GetService(serviceType);
        }
    }
}

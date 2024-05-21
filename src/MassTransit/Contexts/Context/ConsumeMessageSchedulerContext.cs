namespace MassTransit.Context
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class ConsumeMessageSchedulerContext :
        MessageSchedulerContext
    {
        readonly Uri _inputAddress;
        readonly Lazy<IMessageScheduler> _scheduler;

        public ConsumeMessageSchedulerContext(ConsumeContext consumeContext, MessageSchedulerFactory schedulerFactory)
        {
            _inputAddress = consumeContext.ReceiveContext.InputAddress;

            SchedulerFactory = schedulerFactory;

            _scheduler = new Lazy<IMessageScheduler>(() => schedulerFactory(consumeContext));
        }

        public MessageSchedulerFactory SchedulerFactory { get; }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message,
            CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken);
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        public Task<ScheduledMessage> ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken);
        }

        public Task<ScheduledMessage> ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        public Task<ScheduledMessage> ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        public Task<ScheduledMessage> ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType,
            IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(destinationAddress, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values,
            CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.ScheduleSend<T>(destinationAddress, scheduledTime, values, cancellationToken);
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.ScheduleSend(destinationAddress, scheduledTime, values, pipe, cancellationToken);
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.ScheduleSend<T>(destinationAddress, scheduledTime, values, pipe, cancellationToken);
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(DateTime scheduledTime, T message, CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.ScheduleSend(_inputAddress, scheduledTime, message, cancellationToken);
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.ScheduleSend(_inputAddress, scheduledTime, message, pipe, cancellationToken);
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(DateTime scheduledTime, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.ScheduleSend(_inputAddress, scheduledTime, message, pipe, cancellationToken);
        }

        public Task<ScheduledMessage> ScheduleSend(DateTime scheduledTime, object message, CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(_inputAddress, scheduledTime, message, cancellationToken);
        }

        public Task<ScheduledMessage> ScheduleSend(DateTime scheduledTime, object message, Type messageType,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(_inputAddress, scheduledTime, message, messageType, cancellationToken);
        }

        public Task<ScheduledMessage> ScheduleSend(DateTime scheduledTime, object message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(_inputAddress, scheduledTime, message, pipe, cancellationToken);
        }

        public Task<ScheduledMessage> ScheduleSend(DateTime scheduledTime, object message, Type messageType, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(_inputAddress, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(DateTime scheduledTime, object values, CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.ScheduleSend<T>(_inputAddress, scheduledTime, values, cancellationToken);
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.ScheduleSend(_inputAddress, scheduledTime, values, pipe, cancellationToken);
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(DateTime scheduledTime, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.ScheduleSend<T>(_inputAddress, scheduledTime, values, pipe, cancellationToken);
        }

        Task IMessageScheduler.CancelScheduledSend(Uri destinationAddress, Guid tokenId, CancellationToken cancellationToken)
        {
            return _scheduler.Value.CancelScheduledSend(destinationAddress, tokenId, cancellationToken);
        }

        public Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, T message, CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.SchedulePublish(scheduledTime, message, cancellationToken);
        }

        public Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.SchedulePublish(scheduledTime, message, pipe, cancellationToken);
        }

        public Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.SchedulePublish(scheduledTime, message, pipe, cancellationToken);
        }

        public Task<ScheduledMessage> SchedulePublish(DateTime scheduledTime, object message, CancellationToken cancellationToken)
        {
            return _scheduler.Value.SchedulePublish(scheduledTime, message, cancellationToken);
        }

        public Task<ScheduledMessage> SchedulePublish(DateTime scheduledTime, object message, Type messageType, CancellationToken cancellationToken)
        {
            return _scheduler.Value.SchedulePublish(scheduledTime, message, messageType, cancellationToken);
        }

        public Task<ScheduledMessage> SchedulePublish(DateTime scheduledTime, object message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.SchedulePublish(scheduledTime, message, pipe, cancellationToken);
        }

        public Task<ScheduledMessage> SchedulePublish(DateTime scheduledTime, object message, Type messageType, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.SchedulePublish(scheduledTime, message, messageType, pipe, cancellationToken);
        }

        public Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, object values, CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.SchedulePublish<T>(scheduledTime, values, cancellationToken);
        }

        public Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.SchedulePublish(scheduledTime, values, pipe, cancellationToken);
        }

        public Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.SchedulePublish<T>(scheduledTime, values, pipe, cancellationToken);
        }

        public Task CancelScheduledPublish<T>(Guid tokenId, CancellationToken cancellationToken)
            where T : class
        {
            return _scheduler.Value.CancelScheduledPublish<T>(tokenId, cancellationToken);
        }

        public Task CancelScheduledPublish(Type messageType, Guid tokenId, CancellationToken cancellationToken)
        {
            return _scheduler.Value.CancelScheduledPublish(messageType, tokenId, cancellationToken);
        }
    }
}

namespace MassTransit.Context
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Scheduling;


    public class ConsumeMessageSchedulerContext :
        MessageSchedulerContext
    {
        readonly Lazy<IMessageScheduler> _scheduler;
        readonly Uri _inputAddress;

        public ConsumeMessageSchedulerContext(Func<IMessageScheduler> schedulerFactory, Uri inputAddress)
        {
            _inputAddress = inputAddress ?? throw new ArgumentNullException(nameof(inputAddress));

            _scheduler = new Lazy<IMessageScheduler>(schedulerFactory);
        }

        Task IMessageScheduler.CancelScheduledSend(Guid tokenId)
        {
            return _scheduler.Value.CancelScheduledSend(_inputAddress, tokenId);
        }

        public Task CancelScheduledSend(Uri destinationAddress, Guid tokenId)
        {
            return _scheduler.Value.CancelScheduledSend(destinationAddress, tokenId);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType,
            IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(destinationAddress, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend<T>(destinationAddress, scheduledTime, values, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(destinationAddress, scheduledTime, values, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend<T>(destinationAddress, scheduledTime, values, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, T message, CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(_inputAddress, scheduledTime, message, cancellationToken);
        }

        Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(_inputAddress, scheduledTime, message, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(_inputAddress, scheduledTime, message, pipe, cancellationToken);
        }

        Task<ScheduledMessage> MessageSchedulerContext.ScheduleSend(DateTime scheduledTime, object message, CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(_inputAddress, scheduledTime, message, cancellationToken);
        }

        Task<ScheduledMessage> MessageSchedulerContext.ScheduleSend(DateTime scheduledTime, object message, Type messageType,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(_inputAddress, scheduledTime, message, messageType, cancellationToken);
        }

        Task<ScheduledMessage> MessageSchedulerContext.ScheduleSend(DateTime scheduledTime, object message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(_inputAddress, scheduledTime, message, pipe, cancellationToken);
        }

        Task<ScheduledMessage> MessageSchedulerContext.ScheduleSend(DateTime scheduledTime, object message, Type messageType, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(_inputAddress, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, object values, CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend<T>(_inputAddress, scheduledTime, values, cancellationToken);
        }

        Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend(_inputAddress, scheduledTime, values, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            return _scheduler.Value.ScheduleSend<T>(_inputAddress, scheduledTime, values, pipe, cancellationToken);
        }
    }
}

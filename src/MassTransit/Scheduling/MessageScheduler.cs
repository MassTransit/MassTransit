namespace MassTransit.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Initializers;


    public class MessageScheduler :
        IMessageScheduler
    {
        readonly IScheduleMessageProvider _provider;

        public MessageScheduler(IScheduleMessageProvider provider)
        {
            _provider = provider;
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return _provider.ScheduleSend(destinationAddress, scheduledTime, Task.FromResult(message), Pipe.Empty<SendContext>(), cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return _provider.ScheduleSend(destinationAddress, scheduledTime, Task.FromResult(message), pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return _provider.ScheduleSend(destinationAddress, scheduledTime, Task.FromResult(message), pipe, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            var messageType = message.GetType();

            return MessageSchedulerConverterCache.ScheduleSend(this, destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            return MessageSchedulerConverterCache.ScheduleSend(this, destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var messageType = message.GetType();

            return MessageSchedulerConverterCache.ScheduleSend(this, destinationAddress, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType,
            IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return MessageSchedulerConverterCache.ScheduleSend(this, destinationAddress, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var message = MessageInitializerCache<T>.InitializeMessage(values, cancellationToken);

            return _provider.ScheduleSend(destinationAddress, scheduledTime, message, Pipe.Empty<SendContext>(), cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var message = MessageInitializerCache<T>.InitializeMessage(values, cancellationToken);

            return _provider.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var message = MessageInitializerCache<T>.InitializeMessage(values, cancellationToken);

            return _provider.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        public Task CancelScheduledSend(Guid tokenId)
        {
            return _provider.CancelScheduledSend(tokenId);
        }

        public Task CancelScheduledSend(Uri destinationAddress, Guid tokenId)
        {
            return _provider.CancelScheduledSend(destinationAddress, tokenId);
        }
    }
}

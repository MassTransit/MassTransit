namespace MassTransit.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Initializers;


    public class MessageScheduler :
        IMessageScheduler
    {
        readonly IBusTopology _busTopology;
        readonly IScheduleMessageProvider _provider;

        public MessageScheduler(IScheduleMessageProvider provider, IBusTopology busTopology)
        {
            _provider = provider;
            _busTopology = busTopology;
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message,
            CancellationToken cancellationToken)
            where T : class
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return _provider.ScheduleSend(destinationAddress, scheduledTime, message, Pipe.Empty<SendContext>(), cancellationToken);
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return _provider.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return _provider.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        public Task<ScheduledMessage> ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            var messageType = message.GetType();

            return MessageSchedulerConverterCache.ScheduleSend(this, destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        public Task<ScheduledMessage> ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType,
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

        public Task<ScheduledMessage> ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, IPipe<SendContext> pipe,
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

        public Task<ScheduledMessage> ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType,
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

        public async Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values,
            CancellationToken cancellationToken)
            where T : class
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            SendTuple<T> send = await MessageInitializerCache<T>.InitializeMessage(values, cancellationToken).ConfigureAwait(false);

            return await _provider.ScheduleSend(destinationAddress, scheduledTime, send.Message, send.Pipe, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            SendTuple<T> send = await MessageInitializerCache<T>.InitializeMessage(values, pipe, cancellationToken).ConfigureAwait(false);

            return await _provider.ScheduleSend(destinationAddress, scheduledTime, send.Message, send.Pipe, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            SendTuple<T> send = await MessageInitializerCache<T>.InitializeMessage(values, pipe, cancellationToken).ConfigureAwait(false);

            return await _provider.ScheduleSend(destinationAddress, scheduledTime, send.Message, send.Pipe, cancellationToken).ConfigureAwait(false);
        }

        public Task CancelScheduledSend(Uri destinationAddress, Guid tokenId)
        {
            return _provider.CancelScheduledSend(destinationAddress, tokenId);
        }

        public Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, T message, CancellationToken cancellationToken = default)
            where T : class
        {
            var destinationAddress = GetPublishAddress<T>();

            return ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken);
        }

        public Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var destinationAddress = GetPublishAddress<T>();

            return ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        public Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var destinationAddress = GetPublishAddress<T>();

            return ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        public Task<ScheduledMessage> SchedulePublish(DateTime scheduledTime, object message, CancellationToken cancellationToken = default)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            var destinationAddress = GetPublishAddress(messageType);

            return ScheduleSend(destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        public Task<ScheduledMessage> SchedulePublish(DateTime scheduledTime, object message, Type messageType, CancellationToken cancellationToken = default)
        {
            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            var destinationAddress = GetPublishAddress(messageType);

            return ScheduleSend(destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        public Task<ScheduledMessage> SchedulePublish(DateTime scheduledTime, object message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            var destinationAddress = GetPublishAddress(messageType);

            return ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        public Task<ScheduledMessage> SchedulePublish(DateTime scheduledTime, object message, Type messageType, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default)
        {
            var destinationAddress = GetPublishAddress(messageType);

            return ScheduleSend(destinationAddress, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        public Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, object values, CancellationToken cancellationToken = default)
            where T : class
        {
            var destinationAddress = GetPublishAddress<T>();

            return ScheduleSend<T>(destinationAddress, scheduledTime, values, cancellationToken);
        }

        public Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var destinationAddress = GetPublishAddress<T>();

            return ScheduleSend(destinationAddress, scheduledTime, values, pipe, cancellationToken);
        }

        public Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var destinationAddress = GetPublishAddress<T>();

            return ScheduleSend<T>(destinationAddress, scheduledTime, values, pipe, cancellationToken);
        }

        public Task CancelScheduledPublish<T>(Guid tokenId)
            where T : class
        {
            var destinationAddress = GetPublishAddress<T>();

            return CancelScheduledSend(destinationAddress, tokenId);
        }

        public Task CancelScheduledPublish(Type messageType, Guid tokenId)
        {
            var destinationAddress = GetPublishAddress(messageType);

            return CancelScheduledSend(destinationAddress, tokenId);
        }

        Uri GetPublishAddress<T>()
            where T : class
        {
            if (_busTopology.TryGetPublishAddress<T>(out var address))
                return address;

            throw new ArgumentException($"The publish address for the specified type was not returned: {TypeCache<T>.ShortName}");
        }

        Uri GetPublishAddress(Type messageType)
        {
            if (_busTopology.TryGetPublishAddress(messageType, out var address))
                return address;

            throw new ArgumentException($"The publish address for the specified type was not returned: {TypeCache.GetShortName(messageType)}");
        }
    }
}

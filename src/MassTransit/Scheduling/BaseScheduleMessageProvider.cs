namespace MassTransit.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public abstract class BaseScheduleMessageProvider :
        IScheduleMessageProvider
    {
        public async Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            var scheduleMessagePipe = new ScheduleMessageContextPipe<T>(message, pipe);

            var tokenId = ScheduleTokenIdCache<T>.GetTokenId(message);

            scheduleMessagePipe.ScheduledMessageId = tokenId;

            ScheduleMessage command = new ScheduleMessageCommand<T>(scheduledTime, destinationAddress, message, tokenId);

            await ScheduleSend(command, scheduleMessagePipe, cancellationToken).ConfigureAwait(false);

            return new ScheduledMessageHandle<T>(scheduleMessagePipe.ScheduledMessageId ?? command.CorrelationId, command.ScheduledTime,
                command.Destination, message);
        }

        public Task CancelScheduledSend(Guid tokenId)
        {
            return CancelScheduledSend(tokenId, null);
        }

        public Task CancelScheduledSend(Uri destinationAddress, Guid tokenId)
        {
            return CancelScheduledSend(tokenId, destinationAddress);
        }

        protected abstract Task ScheduleSend(ScheduleMessage message, IPipe<SendContext<ScheduleMessage>> pipe, CancellationToken cancellationToken);

        protected abstract Task CancelScheduledSend(Guid tokenId, Uri destinationAddress);
    }


    /// <summary>
    /// For remote endpoint schedulers, used to invoke the <see cref="SendContext{T}" /> pipe and
    /// manage the ScheduledMessageId
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    class ScheduleMessageContextPipe<T> :
        IPipe<SendContext<ScheduleMessage>>
        where T : class
    {
        readonly T _payload;
        readonly IPipe<SendContext<T>> _pipe;
        SendContext _context;

        Guid? _scheduledMessageId;

        public ScheduleMessageContextPipe(T payload, IPipe<SendContext<T>> pipe)
        {
            _payload = payload;
            _pipe = pipe;
        }

        public Guid? ScheduledMessageId
        {
            get => _context?.ScheduledMessageId ?? _scheduledMessageId;
            set => _scheduledMessageId = value;
        }

        public async Task Send(SendContext<ScheduleMessage> context)
        {
            _context = context;

            _context.ScheduledMessageId = _scheduledMessageId;

            if (_pipe.IsNotEmpty())
            {
                SendContext<T> proxy = context.CreateProxy(_payload);

                await _pipe.Send(proxy).ConfigureAwait(false);
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe?.Probe(context);
        }
    }
}

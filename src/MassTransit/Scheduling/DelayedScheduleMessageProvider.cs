namespace MassTransit.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class DelayedScheduleMessageProvider :
        IScheduleMessageProvider
    {
        readonly ISendEndpointProvider _sendEndpointProvider;

        public DelayedScheduleMessageProvider(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            var scheduleMessagePipe = new ScheduleSendPipe<T>(pipe, scheduledTime);

            var tokenId = ScheduleTokenIdCache<T>.GetTokenId(message);

            scheduleMessagePipe.ScheduledMessageId = tokenId;

            var schedulerEndpoint = await _sendEndpointProvider.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await schedulerEndpoint.Send(message, scheduleMessagePipe, cancellationToken).ConfigureAwait(false);

            return new ScheduledMessageHandle<T>(scheduleMessagePipe.ScheduledMessageId ?? NewId.NextGuid(), scheduledTime, destinationAddress, message);
        }

        public Task CancelScheduledSend(Guid tokenId)
        {
            return Task.CompletedTask;
        }

        public Task CancelScheduledSend(Uri destinationAddress, Guid tokenId)
        {
            return Task.CompletedTask;
        }
    }
}

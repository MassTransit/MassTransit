namespace MassTransit.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class ServiceBusScheduleMessageProvider :
        IScheduleMessageProvider
    {
        readonly ISendEndpointProvider _sendEndpointProvider;

        public ServiceBusScheduleMessageProvider(ISendEndpointProvider sendEndpointProvider)
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

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, scheduleMessagePipe, cancellationToken).ConfigureAwait(false);

            return new ScheduledMessageHandle<T>(scheduleMessagePipe.ScheduledMessageId ?? NewId.NextGuid(), scheduledTime, destinationAddress, message);
        }

        public Task CancelScheduledSend(Guid tokenId)
        {
            return Task.CompletedTask;
        }

        public async Task CancelScheduledSend(Uri destinationAddress, Guid tokenId)
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send<CancelScheduledMessage>(new
            {
                InVar.CorrelationId,
                InVar.Timestamp,
                TokenId = tokenId
            }).ConfigureAwait(false);
        }
    }
}

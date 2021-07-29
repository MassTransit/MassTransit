namespace MassTransit.Azure.ServiceBus.Core.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Scheduling;
    using Util;


    public class ServiceBusScheduleMessageProvider :
        IScheduleMessageProvider
    {
        readonly ISendEndpointProvider _sendEndpointProvider;

        public ServiceBusScheduleMessageProvider(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, Task<T> message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            var payload = await message.ConfigureAwait(false);

            var scheduleMessagePipe = new ScheduleSendPipe<T>(pipe, scheduledTime);

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(payload, scheduleMessagePipe, cancellationToken).ConfigureAwait(false);

            return new ScheduledMessageHandle<T>(scheduleMessagePipe.ScheduledMessageId ?? NewId.NextGuid(), scheduledTime, destinationAddress, payload);
        }

        public Task CancelScheduledSend(Guid tokenId)
        {
            return TaskUtil.Completed;
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

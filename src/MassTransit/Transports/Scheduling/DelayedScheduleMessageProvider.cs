namespace MassTransit.Transports.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using MassTransit.Scheduling;
    using Util;


    public class DelayedScheduleMessageProvider :
        IScheduleMessageProvider
    {
        readonly ISendEndpointProvider _sendEndpointProvider;

        public DelayedScheduleMessageProvider(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, Task<T> message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            var payload = await message.ConfigureAwait(false);

            var scheduleMessagePipe = new ScheduleSendPipe<T>(pipe, scheduledTime);

            var tokenId = ScheduleTokenIdCache<T>.GetTokenId(payload);

            scheduleMessagePipe.ScheduledMessageId = tokenId;

            var schedulerEndpoint = await _sendEndpointProvider.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await schedulerEndpoint.Send(payload, scheduleMessagePipe, cancellationToken).ConfigureAwait(false);

            return new ScheduledMessageHandle<T>(scheduleMessagePipe.ScheduledMessageId ?? NewId.NextGuid(), scheduledTime, destinationAddress, payload);
        }

        public Task CancelScheduledSend(Guid tokenId)
        {
            return TaskUtil.Completed;
        }

        public Task CancelScheduledSend(Uri destinationAddress, Guid tokenId)
        {
            return TaskUtil.Completed;
        }
    }
}

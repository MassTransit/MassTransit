namespace MassTransit.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;


    public abstract class BaseScheduleMessageProvider :
        IScheduleMessageProvider
    {
        async Task<ScheduledMessage<T>> IScheduleMessageProvider.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, Task<T> message,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            var payload = await message.ConfigureAwait(false);

            var scheduleMessagePipe = new ScheduleMessageContextPipe<T>(payload, pipe);

            var tokenId = ScheduleTokenIdCache<T>.GetTokenId(payload);

            scheduleMessagePipe.ScheduledMessageId = tokenId;

            ScheduleMessage<T> command = new ScheduleMessageCommand<T>(scheduledTime, destinationAddress, payload, tokenId);

            await ScheduleSend(command, scheduleMessagePipe, cancellationToken).ConfigureAwait(false);

            return new ScheduledMessageHandle<T>(scheduleMessagePipe.ScheduledMessageId ?? command.CorrelationId, command.ScheduledTime,
                command.Destination,
                command.Payload);
        }

        Task IScheduleMessageProvider.CancelScheduledSend(Guid tokenId)
        {
            return CancelScheduledSend(tokenId, null);
        }

        Task IScheduleMessageProvider.CancelScheduledSend(Uri destinationAddress, Guid tokenId)
        {
            return CancelScheduledSend(tokenId, destinationAddress);
        }

        protected abstract Task ScheduleSend(ScheduleMessage message, IPipe<SendContext<ScheduleMessage>> pipe, CancellationToken cancellationToken);

        protected abstract Task CancelScheduledSend(Guid tokenId, Uri destinationAddress);
    }
}

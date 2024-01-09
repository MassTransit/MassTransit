#nullable enable
namespace MassTransit.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using SqlTransport;


    public class SqlScheduleMessageProvider :
        IScheduleMessageProvider
    {
        readonly ConsumeContext _context;

        public SqlScheduleMessageProvider(ConsumeContext context)
        {
            _context = context;
        }

        public async Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            var schedulePipe = new ScheduleSendPipe<T>(pipe, scheduledTime);

            var tokenId = ScheduleTokenIdCache<T>.GetTokenId(message);

            schedulePipe.ScheduledMessageId = tokenId;

            var endpoint = await _context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, schedulePipe, cancellationToken).ConfigureAwait(false);

            LogContext.Debug?.Log("SCHED {DestinationAddress} {MessageId} {MessageType} {DeliveryTime:G} {Token}",
                destinationAddress, schedulePipe.MessageId, TypeCache<T>.ShortName, scheduledTime, schedulePipe.ScheduledMessageId);

            return new ScheduledMessageHandle<T>(schedulePipe.ScheduledMessageId ?? NewId.NextGuid(), scheduledTime, destinationAddress, message);
        }

        public async Task CancelScheduledSend(Guid tokenId)
        {
            if (!_context.TryGetPayload(out ClientContext? clientContext))
                throw new ArgumentException("The client context was not available", nameof(_context));

            var deleted = await clientContext.DeleteScheduledMessage(tokenId).ConfigureAwait(false);
            if (deleted)
                LogContext.Debug?.Log("CANCEL {TokenId}", tokenId);
        }

        public async Task CancelScheduledSend(Uri destinationAddress, Guid tokenId)
        {
            if (!_context.TryGetPayload(out ClientContext? clientContext))
                throw new ArgumentException("The client context was not available", nameof(_context));

            var deleted = await clientContext.DeleteScheduledMessage(tokenId).ConfigureAwait(false);
            if (deleted)
                LogContext.Debug?.Log("CANCEL {DestinationAddress} {TokenId}", destinationAddress, tokenId);
        }
    }
}

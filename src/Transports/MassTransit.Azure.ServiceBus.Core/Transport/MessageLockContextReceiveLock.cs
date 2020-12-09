namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using Transports;
    using Util;


    public class MessageLockContextReceiveLock :
        ReceiveLockContext
    {
        readonly MessageLockContext _lockContext;
        readonly ServiceBusReceiveContext _message;

        public MessageLockContextReceiveLock(MessageLockContext lockContext, ServiceBusReceiveContext message)
        {
            _lockContext = lockContext;
            _message = message;
        }

        public Task Complete()
        {
            return _lockContext.Complete();
        }

        public async Task Faulted(Exception exception)
        {
            if (exception is MessageLockExpiredException)
                return;
            if (exception is MessageTimeToLiveExpiredException)
                return;

            try
            {
                await _lockContext.Abandon(exception).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogContext.Warning?.Log(exception, "Abandon message faulted: {MessageId} - {Exception}", _message.MessageId, ex);
            }
        }

        public Task ValidateLockStatus()
        {
            DateTime now = DateTime.UtcNow;
            if (_message.LockedUntil <= now)
                LogContext.Warning?.Log("Locked: {LockedUntil} <= Now: {Now}, Enqueued: {Enqueued}",
                    _message.LockedUntil,
                    now,
                    _message.EnqueuedTime);

            if (_message.ExpiresAt < now)
                LogContext.Warning?.Log("Expired: {Expire} <= Now: {Now}, Enqueued: {Enqueued}, TTL: {TTL}",
                    _message.ExpiresAt,
                    now,
                    _message.EnqueuedTime,
                    _message.TimeToLive.TotalSeconds);

            return TaskUtil.Completed;
        }
    }
}

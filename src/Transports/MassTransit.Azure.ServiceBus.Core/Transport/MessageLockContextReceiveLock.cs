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
            if (_message.LockedUntil <= DateTime.UtcNow)
                throw new MessageLockExpiredException(_message.InputAddress, $"The message lock expired: {_message.MessageId}");

            if (_message.ExpiresAt < DateTime.UtcNow)
                throw new MessageTimeToLiveExpiredException(_message.InputAddress, $"The message expired: {_message.MessageId}");

            return TaskUtil.Completed;
        }
    }
}

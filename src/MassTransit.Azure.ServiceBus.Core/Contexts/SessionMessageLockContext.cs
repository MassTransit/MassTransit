namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using Util;


    public struct SessionMessageLockContext :
        MessageLockContext
    {
        readonly IMessageSession _session;
        readonly Message _message;

        public SessionMessageLockContext(IMessageSession session, Message message)
        {
            _session = session;
            _message = message;
        }

        public Task Complete()
        {
            return _session.CompleteAsync(_message.SystemProperties.LockToken);
        }

        public Task Abandon(Exception exception)
        {
            return _session.AbandonAsync(_message.SystemProperties.LockToken, ExceptionUtil.GetExceptionHeaderDictionary(exception));
        }
    }
}

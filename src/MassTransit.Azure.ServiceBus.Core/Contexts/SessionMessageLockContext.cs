namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using Util;


    public class SessionMessageLockContext :
        MessageLockContext
    {
        readonly IMessageSession _session;
        readonly Message _message;
        bool _deadLettered;

        public SessionMessageLockContext(IMessageSession session, Message message)
        {
            _session = session;
            _message = message;
        }

        public Task Complete()
        {
            return _deadLettered
                ? TaskUtil.Completed
                : _session.CompleteAsync(_message.SystemProperties.LockToken);
        }

        public Task Abandon(Exception exception)
        {
            return _deadLettered
                ? TaskUtil.Completed
                : _session.AbandonAsync(_message.SystemProperties.LockToken, ExceptionUtil.GetExceptionHeaderDictionary(exception));
        }

        public async Task DeadLetter()
        {
            var headers = new Dictionary<string, object>
            {
                {MessageHeaders.Reason, "dead-letter"},
            };

            await _session.DeadLetterAsync(_message.SystemProperties.LockToken, headers).ConfigureAwait(false);

            _deadLettered = true;
        }

        public async Task DeadLetter(Exception exception)
        {
            await _session.DeadLetterAsync(_message.SystemProperties.LockToken, ExceptionUtil.GetExceptionHeaderDictionary(exception)).ConfigureAwait(false);

            _deadLettered = true;
        }
    }
}

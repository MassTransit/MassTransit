namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;
    using Util;


    public class ReceiverClientMessageLockContext :
        MessageLockContext
    {
        readonly IReceiverClient _receiverClient;
        readonly Message _message;
        bool _deadLettered;

        public ReceiverClientMessageLockContext(IReceiverClient receiverClient, Message message)
        {
            _receiverClient = receiverClient;
            _message = message;
        }

        public Task Complete()
        {
            return _deadLettered
                ? TaskUtil.Completed
                : _receiverClient.CompleteAsync(_message.SystemProperties.LockToken);
        }

        public Task Abandon(Exception exception)
        {
            return _deadLettered
                ? TaskUtil.Completed
                : _receiverClient.AbandonAsync(_message.SystemProperties.LockToken, ExceptionUtil.GetExceptionHeaderDictionary(exception));
        }

        public async Task DeadLetter()
        {
            var headers = new Dictionary<string, object>
            {
                {MessageHeaders.Reason, "dead-letter"},
            };

            await _receiverClient.DeadLetterAsync(_message.SystemProperties.LockToken, headers).ConfigureAwait(false);

            _deadLettered = true;
        }

        public async Task DeadLetter(Exception exception)
        {
            var propertiesToModify = ExceptionUtil.GetExceptionHeaderDictionary(exception);

            await _receiverClient.DeadLetterAsync(_message.SystemProperties.LockToken, propertiesToModify).ConfigureAwait(false);

            _deadLettered = true;
        }
    }
}

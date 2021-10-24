namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::Azure.Messaging.ServiceBus;
    using Util;


    public class ReceiverClientMessageLockContext :
        MessageLockContext
    {
        readonly ServiceBusReceivedMessage _message;
        readonly ProcessMessageEventArgs _receiverClient;
        bool _deadLettered;

        public ReceiverClientMessageLockContext(ProcessMessageEventArgs receiverClient, ServiceBusReceivedMessage message)
        {
            _receiverClient = receiverClient;
            _message = message;
        }

        public Task Complete()
        {
            return _deadLettered
                ? TaskUtil.Completed
                : _receiverClient.CompleteMessageAsync(_message);
        }

        public Task Abandon(Exception exception)
        {
            return _deadLettered
                ? TaskUtil.Completed
                : _receiverClient.AbandonMessageAsync(_message, ExceptionUtil.GetExceptionHeaderDictionary(exception));
        }

        public async Task DeadLetter()
        {
            var headers = new Dictionary<string, object> {{MessageHeaders.Reason, "dead-letter"}};

            await _receiverClient.DeadLetterMessageAsync(_message, headers).ConfigureAwait(false);

            _deadLettered = true;
        }

        public async Task DeadLetter(Exception exception)
        {
            IDictionary<string, object> propertiesToModify = ExceptionUtil.GetExceptionHeaderDictionary(exception);

            await _receiverClient.DeadLetterMessageAsync(_message, propertiesToModify).ConfigureAwait(false);

            _deadLettered = true;
        }
    }
}

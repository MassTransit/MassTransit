namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Util;


    public class ServiceBusSessionMessageLockContext :
        MessageLockContext
    {
        readonly ServiceBusReceivedMessage _message;
        readonly ProcessSessionMessageEventArgs _session;
        bool _deadLettered;

        public ServiceBusSessionMessageLockContext(ProcessSessionMessageEventArgs session, ServiceBusReceivedMessage message)
        {
            _session = session;
            _message = message;
        }

        public Task Complete()
        {
            return _deadLettered
                ? Task.CompletedTask
                : _session.CompleteMessageAsync(_message);
        }

        public Task Abandon(Exception exception)
        {
            return _deadLettered
                ? Task.CompletedTask
                : _session.AbandonMessageAsync(_message, ExceptionUtil.GetExceptionHeaderDictionary(exception));
        }

        public async Task DeadLetter()
        {
            var headers = new Dictionary<string, object> { { MessageHeaders.Reason, "dead-letter" } };

            await _session.DeadLetterMessageAsync(_message, headers).ConfigureAwait(false);

            _deadLettered = true;
        }

        public async Task DeadLetter(Exception exception)
        {
            await _session.DeadLetterMessageAsync(_message, ExceptionUtil.GetExceptionHeaderDictionary(exception)).ConfigureAwait(false);

            _deadLettered = true;
        }
    }
}

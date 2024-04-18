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
            const string reason = "dead-letter";

            var headers = new Dictionary<string, object> { { MessageHeaders.Reason, reason } };

            await _session.DeadLetterMessageAsync(_message, headers, reason).ConfigureAwait(false);

            _deadLettered = true;
        }

        public async Task DeadLetter(Exception exception)
        {
            const string reason = "fault";

            (Dictionary<string, object> dictionary, var message) = ExceptionUtil.GetExceptionHeaderDetail(exception);

            await _session.DeadLetterMessageAsync(_message, dictionary, reason, message).ConfigureAwait(false);

            _deadLettered = true;
        }
    }
}

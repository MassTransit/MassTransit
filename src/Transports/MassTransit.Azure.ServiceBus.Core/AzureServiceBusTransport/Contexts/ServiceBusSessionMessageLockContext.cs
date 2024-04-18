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
            const string deadLetterReason = "dead-letter";
            var headers = new Dictionary<string, object> { { MessageHeaders.Reason, deadLetterReason } };

            await _session.DeadLetterMessageAsync(_message, headers, deadLetterReason).ConfigureAwait(false);

            _deadLettered = true;
        }

        public async Task DeadLetter(Exception exception)
        {
            Dictionary<string, object> exceptionHeaderDictionary = ExceptionUtil.GetExceptionHeaderDictionary(exception);
            var deadLetterReason = (string)exceptionHeaderDictionary[MessageHeaders.Reason];
            var deadLetterErrorDescription = (string)exceptionHeaderDictionary[MessageHeaders.FaultMessage];

            await _session.DeadLetterMessageAsync(_message, exceptionHeaderDictionary, deadLetterReason, deadLetterErrorDescription).ConfigureAwait(false);

            _deadLettered = true;
        }
    }
}

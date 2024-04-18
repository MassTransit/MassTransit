namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Util;


    public class ServiceBusMessageLockContext :
        MessageLockContext
    {
        readonly ProcessMessageEventArgs _eventArgs;
        readonly ServiceBusReceivedMessage _message;
        bool _deadLettered;

        public ServiceBusMessageLockContext(ProcessMessageEventArgs eventArgs, ServiceBusReceivedMessage message)
        {
            _eventArgs = eventArgs;
            _message = message;
        }

        public Task Complete()
        {
            return _deadLettered
                ? Task.CompletedTask
                : _eventArgs.CompleteMessageAsync(_message);
        }

        public Task Abandon(Exception exception)
        {
            return _deadLettered
                ? Task.CompletedTask
                : _eventArgs.AbandonMessageAsync(_message, ExceptionUtil.GetExceptionHeaderDictionary(exception));
        }

        public async Task DeadLetter()
        {
            const string deadLetterReason = "dead-letter";
            var headers = new Dictionary<string, object> { { MessageHeaders.Reason, deadLetterReason } };

            await _eventArgs.DeadLetterMessageAsync(_message, headers).ConfigureAwait(false);

            _deadLettered = true;
        }

        public async Task DeadLetter(Exception exception)
        {
            Dictionary<string, object> exceptionHeaderDictionary = ExceptionUtil.GetExceptionHeaderDictionary(exception);
            var deadLetterReason = (string)exceptionHeaderDictionary[MessageHeaders.Reason];
            var deadLetterErrorDescription = (string)exceptionHeaderDictionary[MessageHeaders.FaultMessage];

            await _eventArgs
                .DeadLetterMessageAsync(_message, exceptionHeaderDictionary, deadLetterReason, deadLetterErrorDescription)
                .ConfigureAwait(false);

            _deadLettered = true;
        }
    }
}

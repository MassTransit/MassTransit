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
            if (_deadLettered)
                return Task.CompletedTask;

            (Dictionary<string, object> dictionary, _) = ExceptionUtil.GetExceptionHeaderDetail(exception, ServiceBusSendTransportContext.Adapter);

            return _eventArgs.AbandonMessageAsync(_message, dictionary);
        }

        public async Task DeadLetter()
        {
            await _eventArgs.DeadLetterMessageAsync(_message, new Dictionary<string, object> { { MessageHeaders.Reason, "dead-letter" } })
                .ConfigureAwait(false);

            _deadLettered = true;
        }

        public async Task DeadLetter(Exception exception)
        {
            (Dictionary<string, object> dictionary, _) = ExceptionUtil.GetExceptionHeaderDetail(exception, ServiceBusSendTransportContext.Adapter);

            await _eventArgs.DeadLetterMessageAsync(_message, dictionary).ConfigureAwait(false);

            _deadLettered = true;
        }
    }
}

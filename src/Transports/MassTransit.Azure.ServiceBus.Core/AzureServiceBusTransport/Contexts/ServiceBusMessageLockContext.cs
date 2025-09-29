namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Util;


    public class ServiceBusMessageLockContext :
        MessageLockContext
    {
        readonly CancellationToken _cancellationToken;
        readonly ProcessMessageEventArgs _eventArgs;
        readonly ServiceBusReceivedMessage _message;
        bool _deadLettered;

        public ServiceBusMessageLockContext(ProcessMessageEventArgs eventArgs, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            _eventArgs = eventArgs;
            _message = message;
            _cancellationToken = cancellationToken;
        }

        public Task Complete()
        {
            return _deadLettered
                ? Task.CompletedTask
                : _eventArgs.CompleteMessageAsync(_message, _cancellationToken);
        }

        public Task Abandon(Exception exception)
        {
            if (_deadLettered)
                return Task.CompletedTask;

            (Dictionary<string, object> dictionary, _) = ExceptionUtil.GetExceptionHeaderDetail(exception, ServiceBusSendTransportContext.Adapter);

            return _eventArgs.AbandonMessageAsync(_message, dictionary, _cancellationToken);
        }

        public async Task DeadLetter()
        {
            await _eventArgs.DeadLetterMessageAsync(_message, new Dictionary<string, object> { { MessageHeaders.Reason, "dead-letter" } }, _cancellationToken)
                .ConfigureAwait(false);

            _deadLettered = true;
        }

        public async Task DeadLetter(Exception exception)
        {
            (Dictionary<string, object> dictionary, _) = ExceptionUtil.GetExceptionHeaderDetail(exception, ServiceBusSendTransportContext.Adapter);

            await _eventArgs.DeadLetterMessageAsync(_message, dictionary, _cancellationToken).ConfigureAwait(false);

            _deadLettered = true;
        }
    }
}

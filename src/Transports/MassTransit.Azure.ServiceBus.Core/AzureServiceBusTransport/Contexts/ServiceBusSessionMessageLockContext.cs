namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Util;


    public class ServiceBusSessionMessageLockContext :
        MessageLockContext
    {
        readonly CancellationToken _cancellationToken;
        readonly ServiceBusReceivedMessage _message;
        readonly ProcessSessionMessageEventArgs _session;
        bool _deadLettered;

        public ServiceBusSessionMessageLockContext(ProcessSessionMessageEventArgs session, ServiceBusReceivedMessage message,
            CancellationToken cancellationToken)
        {
            _session = session;
            _message = message;
            _cancellationToken = cancellationToken;
        }

        public Task Complete()
        {
            return _deadLettered
                ? Task.CompletedTask
                : _session.CompleteMessageAsync(_message, _cancellationToken);
        }

        public Task Abandon(Exception exception)
        {
            if (_deadLettered)
                return Task.CompletedTask;

            (Dictionary<string, object> dictionary, _) = ExceptionUtil.GetExceptionHeaderDetail(exception, ServiceBusSendTransportContext.Adapter);

            return _session.AbandonMessageAsync(_message, dictionary, _cancellationToken);
        }

        public async Task DeadLetter()
        {
            const string reason = "dead-letter";

            var headers = new Dictionary<string, object> { { MessageHeaders.Reason, reason } };

            await _session.DeadLetterMessageAsync(_message, headers, reason, cancellationToken: _cancellationToken).ConfigureAwait(false);

            _deadLettered = true;
        }

        public async Task DeadLetter(Exception exception)
        {
            const string reason = "fault";

            (Dictionary<string, object> dictionary, var message) = ExceptionUtil.GetExceptionHeaderDetail(exception, ServiceBusSendTransportContext.Adapter);

            await _session.DeadLetterMessageAsync(_message, dictionary, reason, message, _cancellationToken).ConfigureAwait(false);

            _deadLettered = true;
        }
    }
}

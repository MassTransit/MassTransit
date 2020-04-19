namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;
    using Transport;


    public class QueueClientContext :
        BasePipeContext,
        ClientContext,
        IAsyncDisposable
    {
        readonly IQueueClient _queueClient;
        readonly ClientSettings _settings;

        public QueueClientContext(ConnectionContext connectionContext, IQueueClient queueClient, Uri inputAddress, ClientSettings settings)
        {
            _queueClient = queueClient;
            _settings = settings;
            ConnectionContext = connectionContext;
            InputAddress = inputAddress;
        }

        public ConnectionContext ConnectionContext { get; }

        public string EntityPath => _queueClient.Path;

        public Uri InputAddress { get; }

        public void OnMessageAsync(Func<IReceiverClient, Message, CancellationToken, Task> callback, Func<ExceptionReceivedEventArgs, Task> exceptionHandler)
        {
            _queueClient.RegisterMessageHandler(async (message, token) =>
            {
                await callback(_queueClient, message, token).ConfigureAwait(false);
            }, _settings.GetOnMessageOptions(exceptionHandler));
        }

        public void OnSessionAsync(Func<IMessageSession, Message, CancellationToken, Task> callback, Func<ExceptionReceivedEventArgs, Task> exceptionHandler)
        {
            _queueClient.RegisterSessionHandler(callback, _settings.GetSessionHandlerOptions(exceptionHandler));
        }

        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_queueClient != null && !_queueClient.IsClosedOrClosing)
                    await _queueClient.CloseAsync().ConfigureAwait(false);

                LogContext.Debug?.Log("Closed client: {InputAddress}", InputAddress);
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Close client faulted: {InputAddress}", InputAddress);
            }
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            return CloseAsync(cancellationToken);
        }
    }
}

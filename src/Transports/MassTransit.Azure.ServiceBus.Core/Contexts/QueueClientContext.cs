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
        bool _unregisterMessageHandler;
        bool _unregisterSessionHandler;

        public QueueClientContext(ConnectionContext connectionContext, IQueueClient queueClient, Uri inputAddress, ClientSettings settings)
        {
            _queueClient = queueClient;
            _settings = settings;
            ConnectionContext = connectionContext;
            InputAddress = inputAddress;
        }

        public ConnectionContext ConnectionContext { get; }

        public string EntityPath => _queueClient.Path;

        public bool IsClosedOrClosing => _queueClient.IsClosedOrClosing || _queueClient.ServiceBusConnection.IsClosedOrClosing;

        public Uri InputAddress { get; }

        public void OnMessageAsync(Func<IReceiverClient, Message, CancellationToken, Task> callback, Func<ExceptionReceivedEventArgs, Task> exceptionHandler)
        {
            _queueClient.RegisterMessageHandler(async (message, token) =>
            {
                await callback(_queueClient, message, token).ConfigureAwait(false);
            }, _settings.GetOnMessageOptions(exceptionHandler));

            _unregisterMessageHandler = true;
        }

        public void OnSessionAsync(Func<IMessageSession, Message, CancellationToken, Task> callback, Func<ExceptionReceivedEventArgs, Task> exceptionHandler)
        {
            _queueClient.RegisterSessionHandler(callback, _settings.GetSessionHandlerOptions(exceptionHandler));

            _unregisterSessionHandler = true;
        }

        public async Task ShutdownAsync()
        {
            try
            {
                if (_queueClient != null && !_queueClient.IsClosedOrClosing)
                {
                    if (_unregisterMessageHandler)
                    {
                        LogContext.Debug?.Log("Shutting down message handler: {InputAddress}", InputAddress);

                        await _queueClient.UnregisterMessageHandlerAsync(_settings.ShutdownTimeout).ConfigureAwait(false);
                    }

                    if (_unregisterSessionHandler)
                    {
                        LogContext.Debug?.Log("Shutting down session handler: {InputAddress}", InputAddress);

                        await _queueClient.UnregisterSessionHandlerAsync(_settings.ShutdownTimeout).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Shutdown client faulted: {InputAddress}", InputAddress);
            }
        }

        public async Task CloseAsync()
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

        public async ValueTask DisposeAsync()
        {
            await CloseAsync().ConfigureAwait(false);
        }
    }
}

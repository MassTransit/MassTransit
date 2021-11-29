namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using MassTransit.Middleware;


    public class QueueClientContext :
        BasePipeContext,
        ClientContext,
        IAsyncDisposable
    {
        readonly IAgent _agent;
        readonly ReceiveSettings _settings;
        ServiceBusProcessor _processor;
        ServiceBusSessionProcessor _sessionProcessor;

        public QueueClientContext(ConnectionContext connectionContext, Uri inputAddress, ReceiveSettings settings, IAgent agent)
        {
            _settings = settings;
            _agent = agent;
            ConnectionContext = connectionContext;
            InputAddress = inputAddress;
        }

        public ConnectionContext ConnectionContext { get; }

        public string EntityPath => _processor?.EntityPath ?? _sessionProcessor?.EntityPath;

        public bool IsClosedOrClosing => _processor?.IsClosed ?? _sessionProcessor?.IsClosed ?? false;

        public Uri InputAddress { get; }

        public void OnMessageAsync(Func<ProcessMessageEventArgs, ServiceBusReceivedMessage, CancellationToken, Task> callback,
            Func<ProcessErrorEventArgs, Task> exceptionHandler)
        {
            if (_processor != null)
                throw new InvalidOperationException("OnMessageAsync can only be called once");
            if (_sessionProcessor != null)
                throw new InvalidOperationException("OnMessageAsync cannot be called with operating on a session");

            _processor = ConnectionContext.CreateQueueProcessor(_settings);

            _processor.ProcessMessageAsync += args => callback(args, args.Message, args.CancellationToken);
            _processor.ProcessErrorAsync += exceptionHandler;
        }

        public void OnSessionAsync(Func<ProcessSessionMessageEventArgs, ServiceBusReceivedMessage, CancellationToken, Task> callback,
            Func<ProcessErrorEventArgs, Task> exceptionHandler)
        {
            if (_sessionProcessor != null)
                throw new InvalidOperationException("OnSessionAsync can only be called once");
            if (_processor != null)
                throw new InvalidOperationException("OnSessionAsync cannot be called with operating without a session");

            _sessionProcessor = ConnectionContext.CreateQueueSessionProcessor(_settings);

            _sessionProcessor.ProcessMessageAsync += args => callback(args, args.Message, args.CancellationToken);
            _sessionProcessor.ProcessErrorAsync += exceptionHandler;
        }

        public async Task StartAsync()
        {
            if (_processor != null)
                await _processor.StartProcessingAsync(CancellationToken).ConfigureAwait(false);

            if (_sessionProcessor != null)
                await _sessionProcessor.StartProcessingAsync(CancellationToken).ConfigureAwait(false);
        }

        public async Task ShutdownAsync()
        {
            try
            {
                if (_processor is { IsClosed: false })
                    await _processor.StopProcessingAsync().ConfigureAwait(false);

                if (_sessionProcessor is { IsClosed: false })
                    await _sessionProcessor.StopProcessingAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Stop processing client faulted: {InputAddress}", InputAddress);
            }
        }

        public async Task CloseAsync()
        {
            try
            {
                if (_processor is { IsClosed: false })
                    await _processor.CloseAsync().ConfigureAwait(false);

                if (_sessionProcessor is { IsClosed: false })
                    await _sessionProcessor.CloseAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Close client faulted: {InputAddress}", InputAddress);
            }
        }

        public Task NotifyFaulted(Exception exception, string entityPath)
        {
            return _agent.Stop($"Unrecoverable exception on {entityPath}");
        }

        public async ValueTask DisposeAsync()
        {
            await CloseAsync().ConfigureAwait(false);
        }
    }
}

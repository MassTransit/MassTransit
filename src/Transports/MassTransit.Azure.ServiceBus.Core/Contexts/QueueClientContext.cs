namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using global::Azure.Messaging.ServiceBus;
    using GreenPipes;
    using GreenPipes.Agents;
    using Transport;


    public class QueueClientContext :
        BasePipeContext,
        ClientContext,
        IAsyncDisposable
    {
        readonly IAgent _agent;
        readonly ServiceBusProcessor _queueClient;
        readonly ServiceBusSessionProcessor _sessionClient;
        bool _processQueueMessages;
        bool _processSessionMessages;

        public QueueClientContext(
            ConnectionContext connectionContext,
            ServiceBusProcessor queueClient,
            ServiceBusSessionProcessor sessionClient,
            Uri inputAddress,
            IAgent agent)
        {
            _queueClient = queueClient;
            _sessionClient = sessionClient;
            _agent = agent;
            ConnectionContext = connectionContext;
            InputAddress = inputAddress;
        }

        public ConnectionContext ConnectionContext { get; }

        public string EntityPath => _queueClient.EntityPath;

        public bool IsClosedOrClosing => _queueClient.IsClosed || _sessionClient.IsClosed;

        public Uri InputAddress { get; }

        public void OnMessageAsync(Func<ProcessMessageEventArgs, ServiceBusReceivedMessage, CancellationToken, Task> callback, Func<ProcessErrorEventArgs, Task> exceptionHandler)
        {
            _queueClient.ProcessMessageAsync += (args) => callback(args, args.Message, args.CancellationToken);
            _queueClient.ProcessErrorAsync += exceptionHandler;

            _processQueueMessages = true;
        }

        public void OnSessionAsync(Func<ProcessSessionMessageEventArgs, ServiceBusReceivedMessage, CancellationToken, Task> callback, Func<ProcessErrorEventArgs, Task> exceptionHandler)
        {
            _sessionClient.ProcessMessageAsync += (args) => callback(args, args.Message, args.CancellationToken);
            _sessionClient.ProcessErrorAsync += exceptionHandler;

            _processSessionMessages = true;
        }

        public async Task StartAsync()
        {
            if (_queueClient != null && _processQueueMessages)
            {
                await _queueClient.StartProcessingAsync();
            }

            if(_sessionClient != null && _processSessionMessages)
            {
                await _sessionClient.StartProcessingAsync();
            }
        }

        public async Task ShutdownAsync()
        {
            try
            {
                if (_queueClient != null && !_queueClient.IsClosed)
                    await _queueClient.StopProcessingAsync().ConfigureAwait(false);

                if (_sessionClient != null && !_sessionClient.IsClosed)
                    await _sessionClient.StopProcessingAsync().ConfigureAwait(false);
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
                if (_queueClient != null && !_queueClient.IsClosed)
                    await _queueClient.CloseAsync().ConfigureAwait(false);

                if (_sessionClient != null && !_sessionClient.IsClosed)
                    await _sessionClient.CloseAsync().ConfigureAwait(false);
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

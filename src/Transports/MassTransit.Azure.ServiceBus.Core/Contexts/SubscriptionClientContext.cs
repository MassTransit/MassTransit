namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using global::Azure.Messaging.ServiceBus;
    using GreenPipes;
    using GreenPipes.Agents;
    using MassTransit.Util;
    using Transport;


    public class SubscriptionClientContext :
        BasePipeContext,
        ClientContext,
        IAsyncDisposable
    {
        readonly IAgent _agent;
        readonly SubscriptionSettings _settings;
        ServiceBusProcessor _queueClient;
        ServiceBusSessionProcessor _sessionClient;

        public SubscriptionClientContext(ConnectionContext connectionContext, Uri inputAddress,
            SubscriptionSettings settings, IAgent agent)
        {
            _settings = settings;
            _agent = agent;

            ConnectionContext = connectionContext;
            InputAddress = inputAddress;
        }

        public ConnectionContext ConnectionContext { get; }

        public string EntityPath => _settings.CreateTopicOptions.Name;

        public bool IsClosedOrClosing => _sessionClient?.IsClosed ?? _queueClient?.IsClosed ?? false;

        public Uri InputAddress { get; }

        public void OnMessageAsync(Func<ProcessMessageEventArgs, ServiceBusReceivedMessage, CancellationToken, Task> callback,
            Func<ProcessErrorEventArgs, Task> exceptionHandler)
        {
            if (_queueClient != null)
                throw new InvalidOperationException("OnMessageAsync can only be called once");
            if (_sessionClient != null)
                throw new InvalidOperationException("OnMessageAsync cannot be called with operating on a session");

            _queueClient = ConnectionContext.CreateSubscriptionProcessor(_settings);

            _queueClient.ProcessMessageAsync += args => callback(args, args.Message, args.CancellationToken);
            _queueClient.ProcessErrorAsync += exceptionHandler;
        }

        public void OnSessionAsync(Func<ProcessSessionMessageEventArgs, ServiceBusReceivedMessage, CancellationToken, Task> callback,
            Func<ProcessErrorEventArgs, Task> exceptionHandler)
        {
            if (_sessionClient != null)
                throw new InvalidOperationException("OnSessionAsync can only be called once");
            if (_queueClient != null)
                throw new InvalidOperationException("OnSessionAsync cannot be called with operating without a session");

            _sessionClient = ConnectionContext.CreateSubscriptionSessionProcessor(_settings);

            _sessionClient.ProcessMessageAsync += args => callback(args, args.Message, args.CancellationToken);
            _sessionClient.ProcessErrorAsync += exceptionHandler;
        }

        public async Task StartAsync()
        {
            if (_queueClient != null)
                await _queueClient.StartProcessingAsync();

            if (_sessionClient != null)
                await _sessionClient.StartProcessingAsync();
        }

        public Task ShutdownAsync()
        {
            // StopProcessingAsync will wait for the message handler(s) to be completed and this could result a deadlock.
            // The current implementation goes against the guidance of managing the state of the client in the handler.
            // https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/servicebus/Azure.Messaging.ServiceBus/src/Processor/ServiceBusProcessor.cs#L318
            // To prevent the deadlock, the call to StopProcessingAsync won't be awaited.

            try
            {
                if (_queueClient is { IsClosed: false })
                    _ = _queueClient.StopProcessingAsync().ConfigureAwait(false);

                if (_sessionClient is { IsClosed: false })
                    _ = _sessionClient.StopProcessingAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Stop processing client faulted: {InputAddress}", InputAddress);
            }

            return TaskUtil.Completed;
        }

        public Task CloseAsync()
        {
            // If an exception occurs that requires the agent to be recycled, this method will be called as part of the processing of
            // a message. CloseAsync will wait for the message handler(s) to be completed and this creates a deadlock.
            // The current implementation goes against the guidance of managing the state of the client in the handler.
            // https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/servicebus/Azure.Messaging.ServiceBus/src/Processor/ServiceBusProcessor.cs#L318
            // To prevent the deadlock, the call to CloseAsync won't be awaited.

            try
            {
                if (_queueClient is { IsClosed: false })
                    _ = _queueClient.CloseAsync().ConfigureAwait(false);

                if (_sessionClient is { IsClosed: false })
                    _ = _sessionClient.CloseAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Close client faulted: {InputAddress}", InputAddress);
            }

            return TaskUtil.Completed;
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

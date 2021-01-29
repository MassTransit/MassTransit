namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using GreenPipes.Agents;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;
    using Transport;


    public class SubscriptionClientContext :
        BasePipeContext,
        ClientContext,
        IAsyncDisposable
    {
        readonly IAgent _agent;
        readonly SubscriptionSettings _settings;
        readonly ISubscriptionClient _subscriptionClient;
        bool _unregisterMessageHandler;
        bool _unregisterSessionHandler;

        public SubscriptionClientContext(ConnectionContext connectionContext, ISubscriptionClient subscriptionClient, Uri inputAddress,
            SubscriptionSettings settings, IAgent agent)
        {
            _subscriptionClient = subscriptionClient;
            _settings = settings;
            _agent = agent;

            ConnectionContext = connectionContext;
            InputAddress = inputAddress;
        }

        public ConnectionContext ConnectionContext { get; }

        public string EntityPath => _settings.TopicDescription.Path;

        public bool IsClosedOrClosing => _subscriptionClient.IsClosedOrClosing || _subscriptionClient.ServiceBusConnection.IsClosedOrClosing;

        public Uri InputAddress { get; }

        public void OnMessageAsync(Func<IReceiverClient, Message, CancellationToken, Task> callback, Func<ExceptionReceivedEventArgs, Task> exceptionHandler)
        {
            _subscriptionClient.RegisterMessageHandler(async (message, token) =>
            {
                await callback(_subscriptionClient, message, token).ConfigureAwait(false);
            }, _settings.GetOnMessageOptions(exceptionHandler));

            _unregisterMessageHandler = true;
        }

        public void OnSessionAsync(Func<IMessageSession, Message, CancellationToken, Task> callback, Func<ExceptionReceivedEventArgs, Task> exceptionHandler)
        {
            _subscriptionClient.RegisterSessionHandler(callback, _settings.GetSessionHandlerOptions(exceptionHandler));

            _unregisterSessionHandler = true;
        }

        public async Task ShutdownAsync()
        {
            try
            {
                if (_subscriptionClient != null && !_subscriptionClient.IsClosedOrClosing)
                {
                    if (_unregisterMessageHandler)
                        await _subscriptionClient.UnregisterMessageHandlerAsync(_settings.ShutdownTimeout).ConfigureAwait(false);

                    if (_unregisterSessionHandler)
                        await _subscriptionClient.UnregisterSessionHandlerAsync(_settings.ShutdownTimeout).ConfigureAwait(false);
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
                if (_subscriptionClient != null && !_subscriptionClient.IsClosedOrClosing)
                    await _subscriptionClient.CloseAsync().ConfigureAwait(false);
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

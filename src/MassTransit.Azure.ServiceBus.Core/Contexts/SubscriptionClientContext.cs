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


    public class SubscriptionClientContext :
        BasePipeContext,
        ClientContext,
        IAsyncDisposable
    {
        readonly ISubscriptionClient _subscriptionClient;
        readonly SubscriptionSettings _settings;

        public SubscriptionClientContext(ISubscriptionClient subscriptionClient, Uri inputAddress, SubscriptionSettings settings)
        {
            _subscriptionClient = subscriptionClient;
            _settings = settings;
            InputAddress = inputAddress;
        }

        public string EntityPath => _settings.TopicDescription.Path;
        public Uri InputAddress { get; }

        public void OnMessageAsync(Func<IReceiverClient, Message, CancellationToken, Task> callback, Func<ExceptionReceivedEventArgs, Task> exceptionHandler)
        {
            _subscriptionClient.RegisterMessageHandler(async (message, token) =>
            {
                await callback(_subscriptionClient, message, token).ConfigureAwait(false);
            }, _settings.GetOnMessageOptions(exceptionHandler));
        }

        public void OnSessionAsync(Func<IMessageSession, Message, CancellationToken, Task> callback, Func<ExceptionReceivedEventArgs, Task> exceptionHandler)
        {
            _subscriptionClient.RegisterSessionHandler(callback, _settings.GetSessionHandlerOptions(exceptionHandler));
        }

        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            LogContext.Debug?.Log("Closing client: {InputAddress}", InputAddress);

            try
            {
                if (_subscriptionClient != null && !_subscriptionClient.IsClosedOrClosing)
                    await _subscriptionClient.CloseAsync().ConfigureAwait(false);

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

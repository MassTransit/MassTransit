namespace MassTransit.Futures.Endpoints
{
    using System.Threading.Tasks;
    using Internals;


    public class FactoryResultEndpoint<TResult, TResponse> :
        IResultEndpoint<TResult>
        where TResult : class
        where TResponse : class
    {
        readonly AsyncFutureMessageFactory<TResult, TResponse> _factory;

        public FactoryResultEndpoint(AsyncFutureMessageFactory<TResult, TResponse> factory)
        {
            _factory = factory;
        }

        public async Task SendResponse(FutureConsumeContext<TResult> context, params FutureSubscription[] subscriptions)
        {
            var response = await context.SetResult(context.Instance.CorrelationId, _factory);

            await context.SendMessageToSubscriptions(subscriptions, response).ConfigureAwait(false);
        }
    }


    public class FactoryResultEndpoint<TResponse> :
        IResultEndpoint
        where TResponse : class
    {
        readonly AsyncFutureMessageFactory<TResponse> _factory;

        public FactoryResultEndpoint(AsyncFutureMessageFactory<TResponse> factory)
        {
            _factory = factory;
        }

        public async Task SendResponse(FutureConsumeContext context, params FutureSubscription[] subscriptions)
        {
            var response = await context.SetResult(context.Instance.CorrelationId, _factory).ConfigureAwait(false);

            await context.SendMessageToSubscriptions(subscriptions, response).ConfigureAwait(false);
        }
    }
}

namespace MassTransit.Futures.Endpoints
{
    using System.Threading.Tasks;
    using Initializers;
    using Internals;


    public class InitializerResultEndpoint<TCommand, TResponse, TResult> :
        IResultEndpoint<TResponse>
        where TCommand : class
        where TResponse : class
        where TResult : class
    {
        readonly InitializerValueProvider<TResponse> _provider;

        public InitializerResultEndpoint(InitializerValueProvider<TResponse> provider)
        {
            _provider = provider;
        }

        public async Task SendResponse(FutureConsumeContext<TResponse> context, params FutureSubscription[] subscriptions)
        {
            context.SetCompleted(context.Instance.CorrelationId);

            InitializeContext<TResult> initializeContext = await MessageInitializerCache<TResult>.Initialize(new
            {
                context.Instance.Completed,
                context.Instance.Created,
                context.Instance.Faulted,
                context.Instance.Location,
            }, context.CancellationToken);

            var request = context.Instance.GetCommand<TCommand>();
            if (request != null)
                initializeContext = await MessageInitializerCache<TResult>.Initialize(initializeContext, request);

            if (context.Message != null)
                initializeContext = await MessageInitializerCache<TResult>.Initialize(initializeContext, context.Message);

            // this is due to the way headers are propagated via the initializer
            var values = _provider(context);

            IMessageInitializer<TResult> initializer = MessageInitializerCache<TResult>.GetInitializer(values.GetType());

            var result = await context.SendMessageToSubscriptions(subscriptions, initializer, initializeContext, values).ConfigureAwait(false);

            if (result == null)
            {
                // initialize the message and save it as the response
                InitializeContext<TResult> messageContext = await initializer.Initialize(initializeContext, values).ConfigureAwait(false);
                result = messageContext.Message;
            }

            context.SetResult(context.Instance.CorrelationId, result);
        }
    }


    public class InitializerResultEndpoint<TCommand, TResult> :
        IResultEndpoint
        where TCommand : class
        where TResult : class
    {
        readonly InitializerValueProvider _provider;

        public InitializerResultEndpoint(InitializerValueProvider provider)
        {
            _provider = provider;
        }

        public async Task SendResponse(FutureConsumeContext context, params FutureSubscription[] subscriptions)
        {
            context.SetCompleted(context.Instance.CorrelationId);

            InitializeContext<TResult> initializeContext = await MessageInitializerCache<TResult>.Initialize(new
            {
                context.Instance.Completed,
                context.Instance.Created,
                context.Instance.Faulted,
                context.Instance.Location,
            }, context.CancellationToken);

            var request = context.Instance.GetCommand<TCommand>();
            if (request != null)
                initializeContext = await MessageInitializerCache<TResult>.Initialize(initializeContext, request);

            // this is due to the way headers are propagated via the initializer
            var values = _provider(context);

            IMessageInitializer<TResult> initializer = MessageInitializerCache<TResult>.GetInitializer(values.GetType());

            var result = await context.SendMessageToSubscriptions(subscriptions, initializer, initializeContext, values).ConfigureAwait(false);

            if (result == null)
            {
                // initialize the message and save it as the response
                InitializeContext<TResult> messageContext = await initializer.Initialize(initializeContext, values).ConfigureAwait(false);
                result = messageContext.Message;
            }

            context.SetResult(context.Instance.CorrelationId, result);
        }
    }
}

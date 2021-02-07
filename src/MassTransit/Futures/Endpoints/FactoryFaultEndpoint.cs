namespace MassTransit.Futures.Endpoints
{
    using System.Threading.Tasks;
    using Internals;


    public class FactoryFaultEndpoint<TResult, TFault> :
        IFaultEndpoint<TResult>
        where TResult : class
        where TFault : class
    {
        readonly AsyncFutureMessageFactory<TResult, TFault> _factory;

        public FactoryFaultEndpoint(AsyncFutureMessageFactory<TResult, TFault> factory)
        {
            _factory = factory;
        }

        public async Task SendFault(FutureConsumeContext<TResult> context, params FutureSubscription[] subscriptions)
        {
            var fault = await _factory(context).ConfigureAwait(false);

            context.SetFault(context.Instance.CorrelationId, fault);

            await context.SendMessageToSubscriptions(subscriptions, fault).ConfigureAwait(false);
        }
    }


    public class FactoryFaultEndpoint<TFault> :
        IFaultEndpoint
        where TFault : class
    {
        readonly AsyncFutureMessageFactory<TFault> _factory;

        public FactoryFaultEndpoint(AsyncFutureMessageFactory<TFault> factory)
        {
            _factory = factory;
        }

        public async Task SendFault(FutureConsumeContext context, params FutureSubscription[] subscriptions)
        {
            var fault = await _factory(context).ConfigureAwait(false);

            context.SetFault(context.Instance.CorrelationId, fault);

            await context.SendMessageToSubscriptions(subscriptions, fault).ConfigureAwait(false);
        }
    }
}
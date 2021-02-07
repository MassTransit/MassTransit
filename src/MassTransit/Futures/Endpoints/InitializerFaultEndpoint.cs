namespace MassTransit.Futures.Endpoints
{
    using System.Threading.Tasks;
    using Initializers;
    using Internals;
    using MassTransit;


    public class InitializerFaultEndpoint<TCommand, TFault, TInput> :
        IFaultEndpoint<TInput>
        where TCommand : class
        where TFault : class
        where TInput : class
    {
        readonly InitializerValueProvider<TInput> _provider;

        public InitializerFaultEndpoint(InitializerValueProvider<TInput> provider)
        {
            _provider = provider;
        }

        public async Task SendFault(FutureConsumeContext<TInput> context, params FutureSubscription[] subscriptions)
        {
            InitializeContext<TFault> initializeContext;
            if (context.Message is Fault fault)
            {
                var request = context.Instance.GetCommand<TCommand>();

                context.SetFaulted(context.Instance.CorrelationId, fault.Timestamp);

                initializeContext = await MessageInitializerCache<TFault>.Initialize(new
                {
                    fault.FaultId,
                    fault.FaultedMessageId,
                    fault.Timestamp,
                    fault.Exceptions,
                    fault.Host,
                    fault.FaultMessageTypes,
                    Message = request
                }, context.CancellationToken);

                initializeContext = await MessageInitializerCache<TFault>.Initialize(initializeContext, context.Message);
            }
            else
            {
                context.SetFaulted(context.Instance.CorrelationId);

                initializeContext = await MessageInitializerCache<TFault>.Initialize(context.Message, context.CancellationToken);
            }

            var values = _provider(context);

            IMessageInitializer<TFault> initializer = MessageInitializerCache<TFault>.GetInitializer(values.GetType());

            InitializeContext<TFault> messageContext = await initializer.Initialize(initializeContext, values).ConfigureAwait(false);

            context.SetFault(context.Instance.CorrelationId, messageContext.Message);

            await context.SendMessageToSubscriptions(subscriptions, initializer, initializeContext, values).ConfigureAwait(false);
        }
    }


    public class InitializerFaultEndpoint<TFault> :
        IFaultEndpoint
        where TFault : class
    {
        readonly InitializerValueProvider _provider;

        public InitializerFaultEndpoint(InitializerValueProvider provider)
        {
            _provider = provider;
        }

        public async Task SendFault(FutureConsumeContext context, params FutureSubscription[] subscriptions)
        {
            context.SetFaulted(context.Instance.CorrelationId);

            var values = _provider(context);

            IMessageInitializer<TFault> initializer = MessageInitializerCache<TFault>.GetInitializer(values.GetType());

            InitializeContext<TFault> initializeContext = initializer.Create(context.CancellationToken);

            InitializeContext<TFault> messageContext = await initializer.Initialize(initializeContext, values).ConfigureAwait(false);

            context.SetFault(context.Instance.CorrelationId, messageContext.Message);

            await context.SendMessageToSubscriptions(subscriptions, initializer, initializeContext, values).ConfigureAwait(false);
        }
    }
}

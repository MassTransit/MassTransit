namespace MassTransit.Conductor.Directory
{
    using Automatonymous;
    using Futures;


    public class ServiceProviderSelector<TInput, TResult> :
        IServiceProviderSelector<TInput, TResult>
        where TResult : class
        where TInput : class
    {
        public IServiceProviderDefinition<TInput, TResult> Consumer<TConsumer>()
            where TConsumer : class, IConsumer<TInput>
        {
            return new RequestEndpointServiceProviderDefinition<TInput, TResult>();
        }

        public IServiceProviderDefinition<TInput, TResult> Factory(MessageFactory<TInput, TResult> factory)
        {
            return new FactoryServiceProviderDefinition<TInput, TResult>(factory);
        }

        public IServiceProviderDefinition<TInput, TResult> Factory(AsyncMessageFactory<TInput, TResult> factory)
        {
            return new FactoryServiceProviderDefinition<TInput, TResult>(factory);
        }

        public IServiceProviderDefinition<TInput, TResult> Initializer(ResultValueProvider<TInput> valueProvider)
        {
            return new InitializerServiceProviderDefinition<TInput, TResult>(valueProvider);
        }

        public IServiceProviderDefinition<TInput, TResult> Future<TFuture>()
            where TFuture : MassTransitStateMachine<FutureState>
        {
            return new RequestEndpointServiceProviderDefinition<TInput, TResult>();
        }
    }
}

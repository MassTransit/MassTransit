namespace MassTransit.Conductor.Directory
{
    using Automatonymous;
    using Futures;


    public interface IServiceProviderSelector<TInput, TResult>
        where TResult : class
        where TInput : class
    {
        /// <summary>
        /// Add a consumer service provider for the input type
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <returns></returns>
        IServiceProviderDefinition<TInput, TResult> Consumer<TConsumer>()
            where TConsumer : class, IConsumer<TInput>;

        /// <summary>
        /// Add a consumer service provider for the input type
        /// </summary>
        /// <returns></returns>
        IServiceProviderDefinition<TInput, TResult> Factory(MessageFactory<TInput, TResult> factory);

        /// <summary>
        /// Add a consumer service provider for the input type
        /// </summary>
        /// <returns></returns>
        IServiceProviderDefinition<TInput, TResult> Factory(AsyncMessageFactory<TInput, TResult> factory);

        /// <summary>
        /// Add a consumer service provider for the input type
        /// </summary>
        /// <returns></returns>
        IServiceProviderDefinition<TInput, TResult> Initializer(ResultValueProvider<TInput> valueProvider);

        /// <summary>
        /// Add a future service provider for the input type
        /// </summary>
        /// <typeparam name="TFuture"></typeparam>
        /// <returns></returns>
        IServiceProviderDefinition<TInput, TResult> Future<TFuture>()
            where TFuture : MassTransitStateMachine<FutureState>;
    }
}

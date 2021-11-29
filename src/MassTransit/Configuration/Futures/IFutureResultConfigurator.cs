namespace MassTransit
{
    public interface IFutureResultConfigurator<TResult, out TInput>
        where TInput : class
        where TResult : class
    {
        /// <summary>
        /// Complete the future using the specified factory method
        /// </summary>
        /// <param name="factoryMethod">Returns the result</param>
        void SetCompletedUsingFactory(EventMessageFactory<FutureState, TInput, TResult> factoryMethod);

        /// <summary>
        /// Complete the future using the specified factory method
        /// </summary>
        /// <param name="factoryMethod">Returns the result</param>
        void SetCompletedUsingFactory(AsyncEventMessageFactory<FutureState, TInput, TResult> factoryMethod);

        /// <summary>
        /// Complete the future using the a message initializer. The initiating command is also used to initialize
        /// result properties prior to apply the values specified.
        /// </summary>
        /// <param name="valueProvider">Returns an object of values to initialize the result</param>
        void SetCompletedUsingInitializer(InitializerValueProvider<TInput> valueProvider);
    }


    public interface IFutureResultConfigurator<TResult>
        where TResult : class
    {
        /// <summary>
        /// Complete the future using the specified factory method
        /// </summary>
        /// <param name="factoryMethod">Returns the result</param>
        void SetCompletedUsingFactory(EventMessageFactory<FutureState, TResult> factoryMethod);

        /// <summary>
        /// Complete the future using the specified factory method
        /// </summary>
        /// <param name="factoryMethod">Returns the result</param>
        void SetCompletedUsingFactory(AsyncEventMessageFactory<FutureState, TResult> factoryMethod);

        /// <summary>
        /// Complete the future using the a message initializer. The initiating command is also used to initialize
        /// result properties prior to apply the values specified.
        /// </summary>
        /// <param name="valueProvider">Returns an object of values to initialize the result</param>
        void SetCompletedUsingInitializer(InitializerValueProvider valueProvider);
    }
}

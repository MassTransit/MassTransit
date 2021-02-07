namespace MassTransit.Futures
{
    public interface IFutureFaultConfigurator<TFault, out TInput>
        where TInput : class
        where TFault : class
    {
        /// <summary>
        /// Fault the future using the specified factory method
        /// </summary>
        /// <param name="factoryMethod">Returns the result</param>
        void SetFaultedUsingFactory(FutureMessageFactory<TInput, TFault> factoryMethod);

        /// <summary>
        /// Fault the future using the specified factory method
        /// </summary>
        /// <param name="factoryMethod">Returns the result</param>
        void SetFaultedUsingFactory(AsyncFutureMessageFactory<TInput, TFault> factoryMethod);

        /// <summary>
        /// Fault the future using the a message initializer. The initiating command is also used to initialize
        /// result properties prior to apply the values specified.
        /// </summary>
        /// <param name="valueProvider">Returns an object of values to initialize the result</param>
        void SetFaultedUsingInitializer(InitializerValueProvider<TInput> valueProvider);
    }


    public interface IFutureFaultConfigurator<TFault>
        where TFault : class
    {
        /// <summary>
        /// Fault the future using the specified factory method
        /// </summary>
        /// <param name="factoryMethod">Returns the result</param>
        void SetFaultedUsingFactory(FutureMessageFactory<TFault> factoryMethod);

        /// <summary>
        /// Fault the future using the specified factory method
        /// </summary>
        /// <param name="factoryMethod">Returns the result</param>
        void SetFaultedUsingFactory(AsyncFutureMessageFactory<TFault> factoryMethod);

        /// <summary>
        /// Fault the future using the a message initializer. The initiating command is also used to initialize
        /// result properties prior to apply the values specified.
        /// </summary>
        /// <param name="valueProvider">Returns an object of values to initialize the result</param>
        void SetFaultedUsingInitializer(InitializerValueProvider valueProvider);
    }
}

namespace MassTransit
{
    using System;
    using Configuration;


    public static class CircuitBreakerConfigurationExtensions
    {
        /// <summary>
        /// Puts a circuit breaker in the pipe, which can automatically prevent the flow of messages to the consumer
        /// when the circuit breaker is opened.
        /// </summary>
        /// <typeparam name="T">The pipe context type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void UseCircuitBreaker<T>(this IPipeConfigurator<T> configurator, Action<ICircuitBreakerConfigurator<T>> configure = null)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new CircuitBreakerPipeSpecification<T>();

            configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }
    }
}

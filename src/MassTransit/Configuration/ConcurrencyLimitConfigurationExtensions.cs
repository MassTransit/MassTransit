namespace MassTransit
{
    using System;
    using Configuration;
    using Contracts;
    using Middleware;


    public static class ConcurrencyLimitConfigurationExtensions
    {
        /// <summary>
        /// Specify a concurrency limit for tasks executing through the filter. No more than the specified
        /// number of tasks will be allowed to execute concurrently.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="concurrencyLimit">The concurrency limit for the subsequent filters in the pipeline</param>
        /// <param name="router">A control pipe to support runtime adjustment</param>
        public static void UseConcurrencyLimit<T>(this IPipeConfigurator<T> configurator, int concurrencyLimit, IPipeRouter router = null)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new ConcurrencyLimitPipeSpecification<T>(concurrencyLimit, router);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Limits the number of concurrent messages consumed on the receive endpoint, regardless of message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="concurrentMessageLimit">The concurrency limit for the subsequent filters in the pipeline</param>
        public static void UseConcurrencyLimit(this IConsumePipeConfigurator configurator, int concurrentMessageLimit)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new ConcurrencyLimitConfigurationObserver(configurator, concurrentMessageLimit);
        }

        /// <summary>
        /// Limits the number of concurrent messages consumed on the receive endpoint, regardless of message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="concurrentMessageLimit">The concurrency limit for the subsequent filters in the pipeline</param>
        /// <param name="managementEndpointConfigurator">A management endpoint configurator to support runtime adjustment</param>
        /// <param name="id">An identifier for the concurrency limit to allow selective adjustment</param>
        public static void UseConcurrencyLimit(this IConsumePipeConfigurator configurator, int concurrentMessageLimit,
            IReceiveEndpointConfigurator managementEndpointConfigurator, string id = default)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new ConcurrencyLimitConfigurationObserver(configurator, concurrentMessageLimit, id);

            managementEndpointConfigurator.Instance(observer.Limiter, x =>
            {
                x.UseConcurrentMessageLimit(1);
                x.Message<SetConcurrencyLimit>(m => m.UseRetry(r => r.None()));
            });
        }
    }
}

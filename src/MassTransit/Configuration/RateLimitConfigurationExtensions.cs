namespace MassTransit
{
    using System;
    using Configuration;
    using Middleware;


    public static class RateLimitConfigurationExtensions
    {
        /// <summary>
        /// Specify a rate limit for message processing, so that only the specified number of messages are allowed
        /// per interval.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="rateLimit">The number of messages allowed per interval</param>
        /// <param name="router">The control pipe used to adjust the rate limit dynamically</param>
        public static void UseRateLimit<T>(this IPipeConfigurator<T> configurator, int rateLimit, IPipeRouter router = null)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new RateLimitPipeSpecification<T>(rateLimit, TimeSpan.FromSeconds(1), router);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Specify a rate limit for message processing, so that only the specified number of messages are allowed
        /// per interval.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="rateLimit">The number of messages allowed per interval</param>
        /// <param name="interval">The reset interval for each set of messages</param>
        /// <param name="router">The control pipe used to adjust the rate limit dynamically</param>
        public static void UseRateLimit<T>(this IPipeConfigurator<T> configurator, int rateLimit, TimeSpan interval, IPipeRouter router = null)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new RateLimitPipeSpecification<T>(rateLimit, interval, router);

            configurator.AddPipeSpecification(specification);
        }
    }
}

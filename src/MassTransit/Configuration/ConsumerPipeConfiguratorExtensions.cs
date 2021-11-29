namespace MassTransit
{
    using System;
    using Configuration;


    public static class ConsumerPipeConfiguratorExtensions
    {
        /// <summary>
        /// Adds a filter to the pipe
        /// </summary>
        /// <typeparam name="T">The context type</typeparam>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="filter">The already built pipe</param>
        public static void UseFilter<TConsumer, T>(this IPipeConfigurator<ConsumerConsumeContext<TConsumer, T>> configurator,
            IFilter<ConsumerConsumeContext<TConsumer>> filter)
            where T : class
            where TConsumer : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new ConsumerFilterSpecification<TConsumer, T>(filter);

            configurator.AddPipeSpecification(specification);
        }
    }
}

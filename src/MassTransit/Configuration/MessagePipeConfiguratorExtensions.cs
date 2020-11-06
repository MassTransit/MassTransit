namespace GreenPipes
{
    using System;
    using MassTransit;
    using Specifications;


    public static class MessagePipeConfiguratorExtensions
    {
        /// <summary>
        /// Adds a filter to the pipe for the specific message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="filter">The filter to add</param>
        public static void UseFilter<T>(this IConsumePipeConfigurator configurator, IFilter<ConsumeContext<T>> filter)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new FilterPipeSpecification<ConsumeContext<T>>(filter);

            configurator.AddPipeSpecification(specification);
        }
    }
}

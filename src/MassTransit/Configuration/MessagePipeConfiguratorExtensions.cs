namespace GreenPipes
{
    using System;
    using MassTransit;
    using Specifications;


    public static class MessagePipeConfiguratorExtensions
    {
        /// <summary>
        /// Adds a filter to the consume pipe for the specific message type
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

        /// <summary>
        /// Adds a filter to the send pipe for the specific message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="filter">The filter to add</param>
        public static void UseFilter<T>(this ISendPipeConfigurator configurator, IFilter<SendContext<T>> filter)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new FilterPipeSpecification<SendContext<T>>(filter);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a filter to the publish pipe for the specific message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="filter">The filter to add</param>
        public static void UseFilter<T>(this IPublishPipeConfigurator configurator, IFilter<PublishContext<T>> filter)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new FilterPipeSpecification<PublishContext<T>>(filter);

            configurator.AddPipeSpecification(specification);
        }
    }
}

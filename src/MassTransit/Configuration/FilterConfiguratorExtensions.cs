namespace MassTransit
{
    using System;
    using GreenPipes;
    using GreenPipes.Specifications;


    public static class FilterConfiguratorExtensions
    {
        [Obsolete("Use `UseFilter` instead")]
        /// <summary>
        /// Adds a filter to the send pipeline
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="filter"></param>
        public static void UseSendFilter<T>(this ISendPipeConfigurator configurator, IFilter<SendContext<T>> filter)
            where T : class
        {
            var specification = new FilterPipeSpecification<SendContext<T>>(filter);

            configurator.AddPipeSpecification(specification);
        }

        [Obsolete("Use `UseFilter` instead")]
        /// <summary>
        /// Adds a filter to the send pipeline
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filter"></param>
        public static void UseSendFilter(this ISendPipeConfigurator configurator, IFilter<SendContext> filter)
        {
            var specification = new FilterPipeSpecification<SendContext>(filter);

            configurator.AddPipeSpecification(specification);
        }
    }
}

namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Configuration;


    public static class FilterConfigurationExtensions
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

        /// <summary>
        /// Adds a filter to the pipe
        /// </summary>
        /// <typeparam name="T">The context type</typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="filter">The filter to add</param>
        public static void UseFilter<T>(this IPipeConfigurator<T> configurator, IFilter<T> filter)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new FilterPipeSpecification<T>(filter);

            configurator.AddPipeSpecification(pipeBuilderConfigurator);
        }

        /// <summary>
        /// Adds filters to the pipe
        /// </summary>
        /// <typeparam name="T">The context type</typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="filters">The filters to add</param>
        public static void UseFilters<T>(this IPipeConfigurator<T> configurator, IEnumerable<IFilter<T>> filters)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));

            foreach (IFilter<T> filter in filters)
            {
                var pipeBuilderConfigurator = new FilterPipeSpecification<T>(filter);

                configurator.AddPipeSpecification(pipeBuilderConfigurator);
            }
        }

        /// <summary>
        /// Adds filters to the pipe
        /// </summary>
        /// <typeparam name="T">The context type</typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="filters">The filters to add</param>
        public static void UseFilters<T>(this IPipeConfigurator<T> configurator, params IFilter<T>[] filters)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));

            foreach (IFilter<T> filter in filters)
            {
                var pipeBuilderConfigurator = new FilterPipeSpecification<T>(filter);

                configurator.AddPipeSpecification(pipeBuilderConfigurator);
            }
        }

        /// <summary>
        /// Adds a filter to the pipe which is of a different type than the native pipe context type
        /// </summary>
        /// <typeparam name="TContext">The context type</typeparam>
        /// <typeparam name="TFilter">The filter context type</typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="filter">The filter to add</param>
        /// <param name="contextProvider"></param>
        /// <param name="inputContextProvider"></param>
        public static void UseFilter<TContext, TFilter>(this IPipeConfigurator<TContext> configurator, IFilter<TFilter> filter,
            MergeFilterContextProvider<TContext, TFilter> contextProvider, FilterContextProvider<TFilter, TContext> inputContextProvider)
            where TContext : class, TFilter
            where TFilter : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var filterSpecification = new FilterPipeSpecification<TFilter>(filter);

            var pipeBuilderConfigurator = new SplitFilterPipeSpecification<TContext, TFilter>(filterSpecification, contextProvider, inputContextProvider);

            configurator.AddPipeSpecification(pipeBuilderConfigurator);
        }
    }
}

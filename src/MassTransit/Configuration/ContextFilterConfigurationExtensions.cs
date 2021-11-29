namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Configuration;


    public static class ContextFilterConfigurationExtensions
    {
        /// <summary>
        /// Adds a content filter that uses a delegate to filter the context and only accept messages
        /// which pass the filter specification.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="filter">A filter method that returns true to accept the message, or false to discard it</param>
        public static void UseContextFilter<T>(this IPipeConfigurator<T> configurator, Func<T, Task<bool>> filter)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            var specification = new ContextFilterPipeSpecification<T>(filter);

            configurator.AddPipeSpecification(specification);
        }
    }
}

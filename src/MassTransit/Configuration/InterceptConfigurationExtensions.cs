namespace MassTransit
{
    using System;
    using Configuration;


    public static class InterceptConfigurationExtensions
    {
        /// <summary>
        /// Adds a fork to the pipe, which invokes a separate pipe before passing to the next filter.
        /// </summary>
        /// <typeparam name="T">The context type</typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="pipe">The filter to add</param>
        public static void UseIntercept<T>(this IPipeConfigurator<T> configurator, IPipe<T> pipe)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new InterceptPipeSpecification<T>(pipe);

            configurator.AddPipeSpecification(specification);
        }
    }
}

namespace MassTransit
{
    using System;
    using Configuration;


    public static class ForkConfigurationExtensions
    {
        /// <summary>
        /// Adds a fork to the pipe, which invokes a separate pipe concurrently with the current pipe
        /// </summary>
        /// <typeparam name="T">The context type</typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="pipe">The filter to add</param>
        public static void UseFork<T>(this IPipeConfigurator<T> configurator, IPipe<T> pipe)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new ForkPipeSpecification<T>(pipe);

            configurator.AddPipeSpecification(specification);
        }
    }
}

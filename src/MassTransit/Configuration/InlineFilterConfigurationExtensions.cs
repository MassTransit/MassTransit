namespace MassTransit
{
    using System;
    using Configuration;


    public static class InlineFilterConfigurationExtensions
    {
        /// <summary>
        /// Creates an inline filter using a simple async method
        /// </summary>
        /// <typeparam name="T">The context type</typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="inlineFilterMethod">The inline filter delegate</param>
        public static void UseInlineFilter<T>(this IPipeConfigurator<T> configurator, InlineFilterMethod<T> inlineFilterMethod)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new InlineFilterPipeSpecification<T>(inlineFilterMethod);

            configurator.AddPipeSpecification(pipeBuilderConfigurator);
        }
    }
}

namespace MassTransit
{
    using System;
    using Configuration;
    using Middleware;


    public static class DispatchConfigurationExtensions
    {
        /// <summary>
        /// Adds a dispatch filter to the pipe, which can be used to route traffic
        /// based on the type of the incoming context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="pipeContextProviderFactory"></param>
        /// <param name="configure"></param>
        public static void UseDispatch<T>(this IPipeConfigurator<T> configurator, IPipeContextConverterFactory<T> pipeContextProviderFactory,
            Action<IDispatchConfigurator<T>> configure = null)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new DispatchPipeSpecification<T>(pipeContextProviderFactory);

            configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }
    }
}

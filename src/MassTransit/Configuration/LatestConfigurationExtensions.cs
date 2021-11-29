namespace MassTransit
{
    using System;
    using Configuration;


    public static class LatestConfigurationExtensions
    {
        /// <summary>
        /// Adds a latest value filter to the pipe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void UseLatest<T>(this IPipeConfigurator<T> configurator, Action<ILatestConfigurator<T>> configure = null)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new LatestPipeSpecification<T>();

            configure?.Invoke(pipeBuilderConfigurator);

            configurator.AddPipeSpecification(pipeBuilderConfigurator);
        }
    }
}

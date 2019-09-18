namespace MassTransit.HttpTransport
{
    using System;
    using GreenPipes;


    static class HttpEndpointDefinitionExtensions
    {
        /// <summary>
        /// We may want to have a builder/endpoint context that could store things like management endpoint, etc. to configure
        /// filters and add configuration interfaces for things like concurrency limit and prefetch count
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="definition"></param>
        /// <param name="configure">The callback to invoke after the definition configuration has been applied</param>
        internal static void Apply(this IHttpReceiveEndpointConfigurator configurator, IEndpointDefinition definition,
            Action<IHttpReceiveEndpointConfigurator> configure = null)
        {
            if (definition.ConcurrentMessageLimit.HasValue)
            {
                configurator.UseConcurrencyLimit(definition.ConcurrentMessageLimit.Value);
            }

            definition.Configure(configurator);

            configure?.Invoke(configurator);
        }
    }
}

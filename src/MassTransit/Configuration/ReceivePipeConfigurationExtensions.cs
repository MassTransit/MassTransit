namespace MassTransit
{
    using GreenPipes;
    using Pipeline.Filters;


    public static class ReceivePipeConfigurationExtensions
    {
        /// <summary>
        /// Use the default _skipped transport for messages that are not consumed
        /// </summary>
        /// <param name="configurator"></param>
        public static void ConfigureDefaultDeadLetterTransport(this IReceivePipelineConfigurator configurator)
        {
            configurator.ConfigureDeadLetter(x => x.UseFilter(new DeadLetterTransportFilter()));
        }

        /// <summary>
        /// Generate a <see cref="ReceiveFault"/> event and move the message to the _error transport.
        /// </summary>
        /// <param name="configurator"></param>
        public static void ConfigureDefaultErrorTransport(this IReceivePipelineConfigurator configurator)
        {
            configurator.ConfigureError(x => x.UseFilter(new GenerateFaultFilter()));
            configurator.ConfigureError(x => x.UseFilter(new ErrorTransportFilter()));
        }
    }
}

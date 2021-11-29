namespace MassTransit
{
    using Middleware;


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
        /// Messages that are not consumed should be discarded instead of being moved to _skipped queue
        /// </summary>
        /// <param name="configurator"></param>
        public static void DiscardSkippedMessages(this IReceivePipelineConfigurator configurator)
        {
            configurator.ConfigureDeadLetter(x => x.UseFilter(new DiscardDeadLetterFilter()));
        }

        /// <summary>
        /// Generate a <see cref="ReceiveFault" /> event and move the message to the _error transport.
        /// </summary>
        /// <param name="configurator"></param>
        public static void ConfigureDefaultErrorTransport(this IReceivePipelineConfigurator configurator)
        {
            configurator.ConfigureError(x =>
            {
                x.UseFilter(new GenerateFaultFilter());
                x.UseFilter(new ErrorTransportFilter());
            });
        }

        /// <summary>
        /// Messages that fault should be discarded instead of being moved to the _error queue. Fault events
        /// will still be published.
        /// </summary>
        /// <param name="configurator"></param>
        public static void DiscardFaultedMessages(this IReceivePipelineConfigurator configurator)
        {
            configurator.ConfigureError(x =>
            {
                x.UseFilter(new GenerateFaultFilter());
                x.UseFilter(new DiscardErrorTransportFilter());
            });
        }

        /// <summary>
        /// Messages that fault should throw exceptions, suppressing the default error queue behavior
        /// </summary>
        /// <param name="configurator"></param>
        public static void RethrowFaultedMessages(this IReceivePipelineConfigurator configurator)
        {
            configurator.ConfigureError(x =>
            {
                x.UseFilter(new RethrowErrorTransportFilter());
            });
        }

        /// <summary>
        /// Messages that are not consumed should throw an exception, forcing the default dead letter behavior
        /// </summary>
        /// <param name="configurator"></param>
        public static void ThrowOnSkippedMessages(this IReceivePipelineConfigurator configurator)
        {
            configurator.ConfigureDeadLetter(x => x.UseFilter(new FaultDeadLetterFilter()));
        }
    }
}

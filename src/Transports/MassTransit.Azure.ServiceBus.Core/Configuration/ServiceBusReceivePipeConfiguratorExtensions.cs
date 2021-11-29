namespace MassTransit
{
    using AzureServiceBusTransport.Middleware;
    using Middleware;


    public static class ServiceBusReceivePipeConfiguratorExtensions
    {
        /// <summary>
        /// Configure the receive endpoint to use the Azure dead-letter queue instead of the _skipped queue
        /// </summary>
        /// <param name="configurator"></param>
        public static void ConfigureDeadLetterQueueDeadLetterTransport(this IReceivePipelineConfigurator configurator)
        {
            configurator.ConfigureDeadLetter(x => x.UseFilter(new DeadLetterQueueFilter()));
        }

        /// <summary>
        /// Generate a <see cref="ReceiveFault" /> event and move the message to the Azure dead-letter queue for the queue/subscription
        /// </summary>
        /// <param name="configurator"></param>
        public static void ConfigureDeadLetterQueueErrorTransport(this IReceivePipelineConfigurator configurator)
        {
            configurator.ConfigureError(x => x.UseFilter(new GenerateFaultFilter()));
            configurator.ConfigureError(x => x.UseFilter(new DeadLetterQueueExceptionFilter()));
        }
    }
}

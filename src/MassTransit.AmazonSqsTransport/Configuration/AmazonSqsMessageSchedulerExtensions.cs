namespace MassTransit.AmazonSqsTransport.Configuration
{
    using System;
    using Specifications;


    public static class AmazonSqsMessageSchedulerExtensions
    {
        /// <summary>
        /// Uses the Amazon SQS delayed messages to schedule messages for future delivery. A lightweight
        /// alternative to Quartz, which does not require any storage outside of AmazonSqs.
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseAmazonSqsMessageScheduler(this IBusFactoryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new DelayedMessageSchedulerSpecification();

            configurator.AddPipeSpecification(specification);
        }
    }
}

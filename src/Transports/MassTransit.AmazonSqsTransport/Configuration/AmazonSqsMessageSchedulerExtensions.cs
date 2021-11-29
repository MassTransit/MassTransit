namespace MassTransit
{
    using System;
    using Configuration;


    public static class AmazonSqsMessageSchedulerExtensions
    {
        /// <summary>
        /// Uses the Amazon SQS delayed messages to schedule messages for future delivery. A lightweight
        /// alternative to Quartz, which does not require any storage outside of AmazonSqs.
        /// </summary>
        /// <param name="configurator"></param>
        [Obsolete("Use the transport independent UseDelayedMessageScheduler")]
        public static void UseAmazonSqsMessageScheduler(this IBusFactoryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new DelayedMessageSchedulerSpecification();

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that uses the SQS message delay to schedule messages.
        /// </summary>
        /// <param name="configurator"></param>
        [Obsolete("Use the transport independent AddDelayedMessageScheduler")]
        public static void AddAmazonSqsMessageScheduler(this IRegistrationConfigurator configurator)
        {
            configurator.AddDelayedMessageScheduler();
        }
    }
}

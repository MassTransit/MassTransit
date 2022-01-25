namespace MassTransit
{
    using System;
    using Configuration;


    public static class MessageSchedulerExtensions
    {
        /// <summary>
        /// Specify an endpoint to use for message scheduling
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="schedulerAddress"></param>
        public static void UseMessageScheduler(this IConsumePipeConfigurator configurator, Uri schedulerAddress)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new MessageSchedulerPipeSpecification(schedulerAddress);

            configurator.AddPrePipeSpecification(pipeBuilderConfigurator);
        }

        /// <summary>
        /// Uses Publish (instead of Send) to schedule messages via the Quartz message scheduler. For this to work, a single
        /// queue should be used to schedule all messages. If multiple instances are running, they should be on the same Quartz
        /// cluster.
        /// </summary>
        /// <param name="configurator"></param>
        public static void UsePublishMessageScheduler(this IConsumePipeConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new PublishMessageSchedulerPipeSpecification();

            configurator.AddPrePipeSpecification(pipeBuilderConfigurator);
        }
    }
}

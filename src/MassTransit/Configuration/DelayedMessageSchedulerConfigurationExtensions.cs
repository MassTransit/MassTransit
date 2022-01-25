namespace MassTransit
{
    using System;
    using Configuration;


    public static class DelayedMessageSchedulerConfigurationExtensions
    {
        /// <summary>
        /// Use the built-in transport message delay to schedule messages
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseDelayedMessageScheduler(this IBusFactoryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new DelayedMessageSchedulerSpecification();

            configurator.AddPrePipeSpecification(specification);
        }
    }
}

namespace MassTransit.ActiveMqTransport
{
    using System;
    using Specifications;


    public static class ActiveMqSchedulerExtensions
    {
        /// <summary>
        /// Uses the ActiveMQ schedule messages for future delivery. A lightweight
        /// alternative to Quartz, which does not require any storage outside of ActiveMQ.
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseActiveMqMessageScheduler(this IBusFactoryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new ActiveMqMessageSchedulerSpecification();

            configurator.AddPipeSpecification(specification);
        }
    }
}

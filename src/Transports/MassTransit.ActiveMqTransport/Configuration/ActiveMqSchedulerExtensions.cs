namespace MassTransit.ActiveMqTransport
{
    using System;
    using MassTransit.Configurators;


    public static class ActiveMqSchedulerExtensions
    {
        /// <summary>
        /// Uses the ActiveMQ schedule messages for future delivery. A lightweight
        /// alternative to Quartz, which does not require any storage outside of ActiveMQ.
        /// </summary>
        /// <param name="configurator"></param>
        [Obsolete("Use the transport independent UseDelayedMessageScheduler")]
        public static void UseActiveMqMessageScheduler(this IBusFactoryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new DelayedMessageSchedulerSpecification();

            configurator.AddPipeSpecification(specification);
        }
    }
}

namespace MassTransit
{
    using System;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;


    public class QuartzSchedulerOptions
    {
        /// <summary>
        /// Used to create the scheduler at bus start, defaults to <see cref="StdSchedulerFactory" />.
        /// </summary>
        public ISchedulerFactory SchedulerFactory { get; set; } = new StdSchedulerFactory();

        /// <summary>
        /// The queue name for the quartz service, defaults to "quartz".
        /// </summary>
        public string QueueName { get; set; } = "quartz";

        /// <summary>
        /// Only supported when configuring the in-memory scheduler to inject the MassTransitJobFactory
        /// when not using a container.
        /// </summary>
        public Func<IBus, IJobFactory>? JobFactoryFactory { get; set; }

        /// <summary>
        /// Whether to start the scheduler when bus starts, defaults to true.
        /// </summary>
        public bool StartScheduler { get; set; } = true;
    }
}

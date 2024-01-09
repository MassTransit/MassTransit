namespace MassTransit
{
    using System;


    /// <summary>
    /// Configure a database transport queue
    /// </summary>
    public interface ISqlQueueConfigurator
    {
        /// <summary>
        /// If specified, the queue will be automatically removed after no consumer activity within the specific idle period
        /// </summary>
        public TimeSpan? AutoDeleteOnIdle { set; }
    }
}

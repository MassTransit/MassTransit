namespace MassTransit
{
    using System;


    public interface IServiceBusEntityConfigurator
    {
        /// <summary>
        /// True if the queue should be deleted if idle
        /// </summary>
        TimeSpan? AutoDeleteOnIdle { set; }

        /// <summary>
        /// Set the default message time to live in the queue
        /// </summary>
        TimeSpan? DefaultMessageTimeToLive { set; }

        /// <summary>
        /// Sets a value that indicates whether server-side batched operations are enabled
        /// </summary>
        bool? EnableBatchedOperations { set; }

        /// <summary>
        /// Sets the user metadata.
        /// </summary>
        string UserMetadata { set; }
    }
}

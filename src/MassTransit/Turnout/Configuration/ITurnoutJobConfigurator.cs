namespace MassTransit.Turnout.Configuration
{
    using System;


    public interface ITurnoutJobConfigurator<out TJob>
        where TJob : class
    {
        /// <summary>
        /// The maximum number of jobs allowed to run concurrently (defaults to 1).
        /// </summary>
        int ConcurrentJobLimit { set; }

        /// <summary>
        /// The allowed execution time of time a job before cancellation is attempted (defaults to 30 minutes).
        /// </summary>
        TimeSpan JobTimeout { set; }

        /// <summary>
        /// Sets the job factory, which is used to execute the job
        /// </summary>
        IJobFactory<TJob> JobFactory { set; }
    }
}

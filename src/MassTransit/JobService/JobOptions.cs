namespace MassTransit.JobService
{
    using System;
    using ConsumeConfigurators;


    /// <summary>
    /// JobOptions contains the options used to configure the job consumer and related components
    /// </summary>
    /// <typeparam name="TJob">The Job Type</typeparam>
    public class JobOptions<TJob> :
        IOptions
        where TJob : class
    {
        public JobOptions()
        {
            ConcurrentJobLimit = 1;
            JobTimeout = TimeSpan.FromMinutes(5);
        }

        /// <summary>
        /// The maximum allowed time for the job to execute, per attempt
        /// </summary>
        public TimeSpan JobTimeout { get; set; }

        /// <summary>
        /// Limits the concurrent number of job executing
        /// </summary>
        public int ConcurrentJobLimit { get; set; }

        public JobOptions<TJob> SetJobTimeout(TimeSpan timeout)
        {
            JobTimeout = timeout;

            return this;
        }

        public JobOptions<TJob> SetConcurrentJobLimit(int limit)
        {
            ConcurrentJobLimit = limit;

            return this;
        }
    }
}

namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Observables;


    /// <summary>
    /// JobOptions contains the options used to configure the job consumer and related components
    /// </summary>
    /// <typeparam name="TJob">The Job Type</typeparam>
    public class JobOptions<TJob> :
        IOptions,
        ISpecification
        where TJob : class
    {
        public JobOptions()
        {
            ConcurrentJobLimit = 1;
            JobTimeout = TimeSpan.FromMinutes(5);

            RetryPolicy = Retry.None;
        }

        /// <summary>
        /// The maximum allowed time for the job to execute, per attempt
        /// </summary>
        public TimeSpan JobTimeout { get; set; }

        /// <summary>
        /// Limits the concurrent number of job executing
        /// </summary>
        public int ConcurrentJobLimit { get; set; }

        public IRetryPolicy RetryPolicy { get; private set; }

        public IEnumerable<ValidationResult> Validate()
        {
            if (ConcurrentJobLimit <= 0)
                yield return this.Failure("JobOptions", "ConcurrentJobLimit", "Must be > 0");
            if (JobTimeout <= TimeSpan.Zero)
                yield return this.Failure("JobOptions", "JobTimeout", "Must be > TimeSpan.Zero");
        }

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

        public JobOptions<TJob> SetRetry(Action<IRetryConfigurator> configure)
        {
            var specification = new RetrySpecification();

            configure?.Invoke(specification);

            RetryPolicy = specification.Build();

            return this;
        }


        class RetrySpecification :
            ExceptionSpecification,
            IRetryConfigurator
        {
            readonly RetryObservable _observers;
            RetryPolicyFactory _policyFactory;

            public RetrySpecification()
            {
                _observers = new RetryObservable();
            }

            public void SetRetryPolicy(RetryPolicyFactory factory)
            {
                _policyFactory = factory;
            }

            ConnectHandle IRetryObserverConnector.ConnectRetryObserver(IRetryObserver observer)
            {
                return _observers.Connect(observer);
            }

            public IRetryPolicy Build()
            {
                return _policyFactory(Filter);
            }
        }
    }
}

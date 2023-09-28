#nullable enable
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
        /// Set the allowed time for a job to complete (per attempt). If the job timeout expires and the job has not yet completed, it will be canceled.
        /// </summary>
        public TimeSpan JobTimeout { get; set; }

        /// <summary>
        /// Set the concurrent job limit. The limit is applied to each instance if the job consumer is scaled out.
        /// Do not use ConcurrentMessageLimit with job consumers."/>
        /// </summary>
        public int ConcurrentJobLimit { get; set; }

        public IRetryPolicy RetryPolicy { get; private set; }

        /// <summary>
        /// Override the default job name (optional, automatically generated from the job type otherwise) that is displayed in the <see cref="JobTypeSaga" />.
        /// </summary>
        public string? JobTypeName { get; set; }

        IEnumerable<ValidationResult> ISpecification.Validate()
        {
            if (ConcurrentJobLimit <= 0)
                yield return this.Failure("JobOptions", "ConcurrentJobLimit", "Must be > 0");
            if (JobTimeout <= TimeSpan.Zero)
                yield return this.Failure("JobOptions", "JobTimeout", "Must be > TimeSpan.Zero");
        }

        /// <summary>
        /// Set the allowed time for a job to complete (per attempt). If the job timeout expires and the job has not yet completed, it will be canceled.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public JobOptions<TJob> SetJobTimeout(TimeSpan timeout)
        {
            JobTimeout = timeout;

            return this;
        }

        /// <summary>
        /// Set the concurrent job limit. The limit is applied to each instance if the job consumer is scaled out.
        /// Do not use ConcurrentMessageLimit with job consumers."/>
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public JobOptions<TJob> SetConcurrentJobLimit(int limit)
        {
            ConcurrentJobLimit = limit;

            return this;
        }

        /// <summary>
        /// Override the default job name (optional, automatically generated from the job type otherwise) that is displayed in the <see cref="JobTypeSaga" />.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JobOptions<TJob> SetJobTypeName(string name)
        {
            JobTypeName = name;

            return this;
        }

        /// <summary>
        /// Set the job retry policy, used to handle faulted jobs. Retry middleware on the job consumer endpoint is not used.
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public JobOptions<TJob> SetRetry(Action<IRetryConfigurator>? configure)
        {
            var specification = new RetrySpecification();

            configure?.Invoke(specification);

            specification.Validate().ThrowIfContainsFailure($"The retry policy was not properly configured: JobOptions<{TypeCache<TJob>.ShortName}");

            RetryPolicy = specification.Build();

            return this;
        }


        class RetrySpecification :
            ExceptionSpecification,
            IRetryConfigurator,
            ISpecification
        {
            readonly RetryObservable _observers;
            RetryPolicyFactory? _policyFactory;

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

            public IEnumerable<ValidationResult> Validate()
            {
                if (_policyFactory == null)
                    yield return this.Failure("RetryPolicy", "must not be null");
            }

            public IRetryPolicy Build()
            {
                if (_policyFactory == null)
                    throw new ConfigurationException($"The retry policy was not properly configured: JobOptions<{TypeCache<TJob>.ShortName}");

                return _policyFactory(Filter);
            }
        }
    }
}

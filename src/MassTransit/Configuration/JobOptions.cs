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
            JobCancellationTimeout = TimeSpan.FromSeconds(30);

            RetryPolicy = Retry.None;

            ProgressBuffer = new ProgressBufferSettings();

            JobProperties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            InstanceProperties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Set the allowed time for a job to complete (per attempt). If the job timeout expires and the job has not yet completed, it will be canceled.
        /// </summary>
        public TimeSpan JobTimeout { get; set; }

        /// <summary>
        /// Set the allowed time for a job to stop execution after the cancellation. If the job cancellation timeout expires and the job has not yet completed, it will be
        /// fully canceled.
        /// </summary>
        public TimeSpan JobCancellationTimeout { get; set; }

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

        /// <summary>
        /// Configure the job progress buffer settings, if using job progress (optional)
        /// </summary>
        public ProgressBufferSettings ProgressBuffer { get; }

        /// <summary>
        /// Properties that are specific to the job type, which can be used by the job distribution strategy
        /// </summary>
        public Dictionary<string, object> JobProperties { get; }

        /// <summary>
        /// Properties that are specific to this job consumer bus instance, such as region, data center, tenant, etc. also used by the job distribution strategy
        /// </summary>
        public Dictionary<string, object> InstanceProperties { get; }

        /// <summary>
        /// Optional, if specified, configures a global concurrent job limit across all job consumer instances
        /// </summary>
        public int? GlobalConcurrentJobLimit { get; set; }

        IEnumerable<ValidationResult> ISpecification.Validate()
        {
            if (ConcurrentJobLimit <= 0)
                yield return this.Failure("JobOptions", "ConcurrentJobLimit", "Must be > 0");
            if (JobTimeout <= TimeSpan.Zero)
                yield return this.Failure("JobOptions", "JobTimeout", "Must be > TimeSpan.Zero");
            if (JobCancellationTimeout <= TimeSpan.Zero)
                yield return this.Failure("JobOptions", "JobCancellationTimeout", "Must be > TimeSpan.Zero");
        }

        /// <summary>
        /// Sets job properties that can be used by a custom job distribution strategy
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public JobOptions<TJob> SetJobProperties(IEnumerable<KeyValuePair<string, object?>>? properties, bool overwrite = true)
        {
            if (properties != null)
            {
                foreach (KeyValuePair<string, object?> property in properties)
                    SetJobProperty(property.Key, property.Value, overwrite);
            }

            return this;
        }

        /// <summary>
        /// Sets a job property that can be used by a custom job distribution strategy
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public JobOptions<TJob> SetJobProperty(string key, string? value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                JobProperties.Remove(key);
            else
                JobProperties[key] = value;

            return this;
        }

        /// <summary>
        /// Sets a job property that can be used by a custom job distribution strategy
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="overwrite"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public JobOptions<TJob> SetJobProperty(string key, object? value, bool overwrite = true)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (overwrite)
            {
                if (value == null)
                    JobProperties.Remove(key);
                else
                    JobProperties[key] = value;
            }
            else if (!JobProperties.ContainsKey(key) && value != null)
                JobProperties.Add(key, value);

            return this;
        }

        /// <summary>
        /// Sets job properties that can be used by a custom job distribution strategy
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public JobOptions<TJob> SetInstanceProperties(IEnumerable<KeyValuePair<string, object?>>? properties, bool overwrite = true)
        {
            if (properties != null)
            {
                foreach (KeyValuePair<string, object?> property in properties)
                    SetInstanceProperty(property.Key, property.Value, overwrite);
            }

            return this;
        }

        /// <summary>
        /// Sets a job property that can be used by a custom job distribution strategy
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public JobOptions<TJob> SetInstanceProperty(string key, string? value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                InstanceProperties.Remove(key);
            else
                InstanceProperties[key] = value;

            return this;
        }

        /// <summary>
        /// Sets a job property that can be used by a custom job distribution strategy
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="overwrite"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public JobOptions<TJob> SetInstanceProperty(string key, object? value, bool overwrite = true)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (overwrite)
            {
                if (value == null)
                    InstanceProperties.Remove(key);
                else
                    InstanceProperties[key] = value;
            }
            else if (!InstanceProperties.ContainsKey(key) && value != null)
                InstanceProperties.Add(key, value);

            return this;
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
        /// Set the allowed time for a job to stop execution after the cancellation. If the job cancellation timeout expires and the job has not yet completed, it will be
        /// fully canceled.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public JobOptions<TJob> SetJobCancellationTimeout(TimeSpan timeout)
        {
            JobCancellationTimeout = timeout;

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
        /// Set the global concurrent job limit across all job consumer instances
        /// Do not use ConcurrentMessageLimit with job consumers."/>
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public JobOptions<TJob> SetGlobalConcurrentJobLimit(int? limit)
        {
            GlobalConcurrentJobLimit = limit;

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

        /// <summary>
        /// Set the job progress buffer settings, either value can be set and will update the settings
        /// </summary>
        /// <param name="updateLimit">The number of updates to buffer before sending the most recent update to the job saga</param>
        /// <param name="timeLimit">The time since the first update after the last update sent to the job saga before an update must be sent</param>
        /// <returns></returns>
        public JobOptions<TJob> SetProgressBuffer(int? updateLimit = default, TimeSpan? timeLimit = default)
        {
            if (updateLimit.HasValue)
                ProgressBuffer.UpdateLimit = updateLimit.Value;
            if (timeLimit.HasValue)
                ProgressBuffer.TimeLimit = timeLimit.Value;

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

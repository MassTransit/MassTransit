#nullable enable
namespace MassTransit;

using System;
using System.Collections.Generic;
using Configuration;
using Observables;
using Serialization;


/// <summary>
/// JobOptions contains the options used to configure the job consumer and related components
/// </summary>
/// <typeparam name="TJob">The Job Type</typeparam>
public class JobOptions<TJob> :
    IOptions,
    ISpecification
    where TJob : class
{
    readonly JobPropertyCollection _instanceProperties;
    readonly JobPropertyCollection _jobTypeProperties;

    public JobOptions()
    {
        ConcurrentJobLimit = 1;
        JobTimeout = TimeSpan.FromMinutes(5);
        JobCancellationTimeout = TimeSpan.FromSeconds(30);

        RetryPolicy = Retry.None;

        ProgressBuffer = new ProgressBufferSettings();

        _jobTypeProperties = new JobPropertyCollection();
        _instanceProperties = new JobPropertyCollection();
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
    public JobPropertyCollection JobTypeProperties => _jobTypeProperties;

    /// <summary>
    /// Properties that are specific to this job consumer bus instance, such as region, data center, tenant, etc. also used by the job distribution strategy
    /// </summary>
    public JobPropertyCollection InstanceProperties => _instanceProperties;

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
    /// Set job type properties that can be used by a custom job distribution strategy
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public JobOptions<TJob> SetJobTypeProperties(Action<ISetPropertyCollection>? callback)
    {
        callback?.Invoke(_jobTypeProperties);

        return this;
    }

    /// <summary>
    /// Set instance properties that can be used by a custom job distribution strategy
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public JobOptions<TJob> SetInstanceProperties(Action<ISetPropertyCollection>? callback)
    {
        callback?.Invoke(_instanceProperties);

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

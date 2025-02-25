namespace MassTransit;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;


public interface JobContext :
    PipeContext,
    MessageContext,
    ISendEndpointProvider,
    IPublishEndpoint
{
    Guid JobId { get; }
    Guid AttemptId { get; }

    /// <summary>
    /// If previously attempted, this value is > 0
    /// </summary>
    int RetryAttempt { get; }

    /// <summary>
    /// The last reported progress value for this job
    /// </summary>
    long? LastProgressValue { get; }

    /// <summary>
    /// The last reported progress limit for this job
    /// </summary>
    long? LastProgressLimit { get; }

    /// <summary>
    /// How long the job has been running
    /// </summary>
    TimeSpan ElapsedTime { get; }

    /// <summary>
    /// The job properties that were supplied when the job was submitted
    /// </summary>
    IPropertyCollection JobProperties { get; }

    /// <summary>
    /// Properties that were configured for this job type
    /// </summary>
    IPropertyCollection JobTypeProperties { get; }

    /// <summary>
    /// Properties that were configured for this job consumer instance
    /// </summary>
    IPropertyCollection InstanceProperties { get; }

    /// <summary>
    /// Sets the job's progress, which gets reported back to the job saga
    /// </summary>
    /// <param name="value"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    Task SetJobProgress(long value, long? limit);

    /// <summary>
    /// Save job state, typically when canceling or faulting, so that subsequent retries can resume from the saved state
    /// </summary>
    /// <param name="jobState"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task SaveJobState<T>(T? jobState)
        where T : class;

    bool TryGetJobState<T>([NotNullWhen(true)] out T? jobState)
        where T : class;
}


public interface JobContext<out TMessage> :
    JobContext
    where TMessage : class
{
    /// <summary>
    /// The message that initiated the job
    /// </summary>
    TMessage Job { get; }
}

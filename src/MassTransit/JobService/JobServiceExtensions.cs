#nullable enable
namespace MassTransit;

using System;
using System.Threading;
using System.Threading.Tasks;
using Clients;
using Contracts.JobService;
using Initializers;
using JobService.Messages;
using Serialization;


public static class JobServiceExtensions
{
    /// <summary>
    /// Requests the job state for the specified <paramref name="jobId" /> using the request client
    /// </summary>
    /// <param name="client"></param>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public static async Task<JobState> GetJobState(this IRequestClient<GetJobState> client, Guid jobId)
    {
        Response<JobState> response = await client.GetResponse<JobState>(new GetJobStateRequest { JobId = jobId });

        return response.Message;
    }

    /// <summary>
    /// Requests the job state for the specified <paramref name="jobId" /> using the request client
    /// </summary>
    /// <param name="client"></param>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public static async Task<JobState<T>> GetJobState<T>(this IRequestClient<GetJobState> client, Guid jobId)
        where T : class
    {
        Response<JobState> response = await client.GetResponse<JobState>(new GetJobStateRequest { JobId = jobId });

        if (response is MessageResponse<JobState> messageResponse)
            return new JobStateResponse<T>(response.Message, messageResponse.DeserializeObject<T>(response.Message.JobState));

        return new JobStateResponse<T>(response.Message);
    }

    /// <summary>
    /// Submits a job, returning the generated jobId
    /// </summary>
    /// <param name="publishEndpoint"></param>
    /// <param name="job"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Task<Guid> SubmitJob<T>(this IPublishEndpoint publishEndpoint, T job, CancellationToken cancellationToken = default)
        where T : class
    {
        return SubmitJob(publishEndpoint, NewId.NextGuid(), job, null, cancellationToken);
    }

    /// <summary>
    /// Submits a job, returning the generated jobId
    /// </summary>
    /// <param name="publishEndpoint"></param>
    /// <param name="job"></param>
    /// <param name="setJobProperties"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Task<Guid> SubmitJob<T>(this IPublishEndpoint publishEndpoint, T job, Action<ISetPropertyCollection>? setJobProperties = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        return SubmitJob(publishEndpoint, NewId.NextGuid(), job, setJobProperties, cancellationToken);
    }

    /// <summary>
    /// Submits a job, returning the generated jobId
    /// </summary>
    /// <param name="publishEndpoint"></param>
    /// <param name="jobId">A unique job id</param>
    /// <param name="job"></param>
    /// <param name="setJobProperties"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<Guid> SubmitJob<T>(this IPublishEndpoint publishEndpoint, Guid jobId, T job, Action<ISetPropertyCollection>?
        setJobProperties = null, CancellationToken cancellationToken = default)
        where T : class
    {
        if (jobId == Guid.Empty)
            throw new ArgumentException("An empty Guid cannot be used as a JobId", nameof(jobId));

        await publishEndpoint.Publish<SubmitJob<T>>(CreateSubmitJobCommand(jobId, job, setJobProperties), cancellationToken).ConfigureAwait(false);

        return jobId;
    }

    static SubmitJobCommand<T> CreateSubmitJobCommand<T>(Guid jobId, T job, Action<ISetPropertyCollection>? setJobProperties)
        where T : class
    {
        var command = new SubmitJobCommand<T>
        {
            JobId = jobId,
            Job = job
        };

        if (setJobProperties != null)
        {
            var properties = new JobPropertyCollection();
            setJobProperties(properties);

            if (properties.Count > 0)
                command.Properties = properties;
        }

        return command;
    }

    /// <summary>
    /// Submits a job, returning the generated jobId
    /// </summary>
    /// <param name="publishEndpoint"></param>
    /// <param name="job"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Task<Guid> SubmitJob<T>(this IPublishEndpoint publishEndpoint, object job, CancellationToken cancellationToken = default)
        where T : class
    {
        return SubmitJob<T>(publishEndpoint, NewId.NextGuid(), job, null, cancellationToken);
    }

    /// <summary>
    /// Submits a job, returning the generated jobId
    /// </summary>
    /// <param name="publishEndpoint"></param>
    /// <param name="job"></param>
    /// <param name="setJobProperties"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Task<Guid> SubmitJob<T>(this IPublishEndpoint publishEndpoint, object job, Action<ISetPropertyCollection>? setJobProperties = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        return SubmitJob<T>(publishEndpoint, NewId.NextGuid(), job, setJobProperties, cancellationToken);
    }

    /// <summary>
    /// Submits a job, returning the generated jobId
    /// </summary>
    /// <param name="publishEndpoint"></param>
    /// <param name="jobId">A unique job id</param>
    /// <param name="job"></param>
    /// <param name="setJobProperties"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<Guid> SubmitJob<T>(this IPublishEndpoint publishEndpoint, Guid jobId, object job, Action<ISetPropertyCollection>?
        setJobProperties = null, CancellationToken cancellationToken = default)
        where T : class
    {
        if (jobId == Guid.Empty)
            throw new ArgumentException("An empty Guid cannot be used as a JobId", nameof(jobId));

        InitializeContext<T> context = await MessageInitializerCache<T>.Initialize(job, cancellationToken).ConfigureAwait(false);

        await publishEndpoint.Publish<SubmitJob<T>>(CreateSubmitJobCommand(jobId, context.Message, setJobProperties), cancellationToken)
            .ConfigureAwait(false);

        return jobId;
    }

    /// <summary>
    /// Submits a job, returning the accepted jobId
    /// </summary>
    /// <param name="client"></param>
    /// <param name="job"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Task<Guid> SubmitJob<T>(this IRequestClient<SubmitJob<T>> client, T job, CancellationToken cancellationToken = default)
        where T : class
    {
        return SubmitJob(client, NewId.NextGuid(), job, null, cancellationToken);
    }

    /// <summary>
    /// Submits a job, returning the accepted jobId
    /// </summary>
    /// <param name="client"></param>
    /// <param name="job"></param>
    /// <param name="setJobProperties"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Task<Guid> SubmitJob<T>(this IRequestClient<SubmitJob<T>> client, T job, Action<ISetPropertyCollection>? setJobProperties = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        return SubmitJob(client, NewId.NextGuid(), job, setJobProperties, cancellationToken);
    }

    /// <summary>
    /// Submits a job, returning the accepted jobId
    /// </summary>
    /// <param name="client"></param>
    /// <param name="jobId">A unique job id</param>
    /// <param name="job"></param>
    /// <param name="setJobProperties"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<Guid> SubmitJob<T>(this IRequestClient<SubmitJob<T>> client, Guid jobId, T job,
        Action<ISetPropertyCollection>? setJobProperties = null, CancellationToken cancellationToken = default)
        where T : class
    {
        if (jobId == Guid.Empty)
            throw new ArgumentException("An empty Guid cannot be used as a JobId", nameof(jobId));

        Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(CreateSubmitJobCommand(jobId, job, setJobProperties),
            cancellationToken).ConfigureAwait(false);

        return response.Message.JobId;
    }

    /// <summary>
    /// Submits a job, returning the accepted jobId
    /// </summary>
    /// <param name="client"></param>
    /// <param name="job"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Task<Guid> SubmitJob<T>(this IRequestClient<SubmitJob<T>> client, object job, CancellationToken cancellationToken = default)
        where T : class
    {
        return SubmitJob(client, NewId.NextGuid(), job, null, cancellationToken);
    }

    /// <summary>
    /// Submits a job, returning the accepted jobId
    /// </summary>
    /// <param name="client"></param>
    /// <param name="job"></param>
    /// <param name="setJobProperties"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Task<Guid> SubmitJob<T>(this IRequestClient<SubmitJob<T>> client, object job, Action<ISetPropertyCollection>? setJobProperties = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        return SubmitJob(client, NewId.NextGuid(), job, setJobProperties, cancellationToken);
    }

    /// <summary>
    /// Submits a job, returning the accepted jobId
    /// </summary>
    /// <param name="client"></param>
    /// <param name="jobId">Specify an explicit jobId for the job</param>
    /// <param name="job"></param>
    /// <param name="setJobProperties"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<Guid> SubmitJob<T>(this IRequestClient<SubmitJob<T>> client, Guid jobId, object job,
        Action<ISetPropertyCollection>? setJobProperties = null, CancellationToken cancellationToken = default)
        where T : class
    {
        InitializeContext<T> context = await MessageInitializerCache<T>.Initialize(job, cancellationToken).ConfigureAwait(false);

        Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(CreateSubmitJobCommand(jobId, context.Message,
            setJobProperties), cancellationToken).ConfigureAwait(false);

        return response.Message.JobId;
    }

    /// <summary>
    /// Submits a job, returning the accepted jobId
    /// </summary>
    /// <param name="client"></param>
    /// <param name="job"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<Guid> SubmitJob<T>(this IRequestClient<T> client, T job, CancellationToken cancellationToken = default)
        where T : class
    {
        Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(job, cancellationToken).ConfigureAwait(false);

        return response.Message.JobId;
    }

    /// <summary>
    /// Submits a job, returning the accepted jobId
    /// </summary>
    /// <param name="client"></param>
    /// <param name="job"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<Guid> SubmitJob<T>(this IRequestClient<T> client, object job, CancellationToken cancellationToken = default)
        where T : class
    {
        Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(job, cancellationToken).ConfigureAwait(false);

        return response.Message.JobId;
    }

    /// <summary>
    /// Cancel a job if the job exists and is in a state that can be canceled.
    /// </summary>
    /// <param name="publishEndpoint"></param>
    /// <param name="jobId"></param>
    /// <param name="reason"></param>
    /// <returns></returns>
    public static Task CancelJob(this IPublishEndpoint publishEndpoint, Guid jobId, string? reason = null)
    {
        return publishEndpoint.Publish<CancelJob>(new CancelJobCommand
        {
            JobId = jobId,
            Reason = reason ?? "Unspecified"
        });
    }

    /// <summary>
    /// Retry a job if the job exists and is in a state that can be retried.
    /// </summary>
    /// <param name="publishEndpoint"></param>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public static Task RetryJob(this IPublishEndpoint publishEndpoint, Guid jobId)
    {
        return publishEndpoint.Publish<RetryJob>(new RetryJobCommand { JobId = jobId });
    }

    /// <summary>
    /// Finalize a job, removing any faulted job attempts, so that it can be removed from the saga repository
    /// </summary>
    /// <param name="publishEndpoint"></param>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public static Task FinalizeJob(this IPublishEndpoint publishEndpoint, Guid jobId)
    {
        return publishEndpoint.Publish<FinalizeJob>(new FinalizeJobCommand { JobId = jobId });
    }
}

namespace MassTransit;

using System;
using System.Threading;
using System.Threading.Tasks;
using Contracts.JobService;
using Initializers;
using JobService;
using JobService.Messages;


public static class RecurringJobConsumerExtensions
{
    /// <summary>
    /// Add or update a recurring job
    /// </summary>
    /// <param name="client">An existing request client</param>
    /// <param name="jobName"></param>
    /// <param name="job"></param>
    /// <param name="cronExpression">The scheduler cron expression</param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<Guid> AddOrUpdateRecurringJob<T>(this IRequestClient<SubmitJob<T>> client, string jobName, T job, string cronExpression,
        CancellationToken cancellationToken = default)
        where T : class
    {
        if (string.IsNullOrWhiteSpace(jobName))
            throw new ArgumentNullException(nameof(jobName));

        var jobId = JobMetadataCache<T>.GenerateRecurringJobId(jobName);

        Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new
        {
            JobId = jobId,
            Job = job,
            Schedule = new { CronExpression = cronExpression }
        }, cancellationToken).ConfigureAwait(false);

        return response.Message.JobId;
    }

    /// <summary>
    /// Add or update a recurring job
    /// </summary>
    /// <param name="publishEndpoint">An available publish endpoint instance</param>
    /// <param name="jobName"></param>
    /// <param name="job"></param>
    /// <param name="cronExpression">The scheduler cron expression</param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<Guid> AddOrUpdateRecurringJob<T>(this IPublishEndpoint publishEndpoint, string jobName, T job, string cronExpression,
        CancellationToken cancellationToken = default)
        where T : class
    {
        if (string.IsNullOrWhiteSpace(jobName))
            throw new ArgumentNullException(nameof(jobName));

        var jobId = JobMetadataCache<T>.GenerateRecurringJobId(jobName);

        await publishEndpoint.Publish<SubmitJob<T>>(new
        {
            JobId = jobId,
            Job = job,
            Schedule = new { CronExpression = cronExpression }
        }, cancellationToken).ConfigureAwait(false);

        return jobId;
    }

    /// <summary>
    /// Submits a job, returning the generated jobId
    /// </summary>
    /// <param name="publishEndpoint"></param>
    /// <param name="start">The start time for the job</param>
    /// <param name="job"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<Guid> ScheduleJob<T>(this IPublishEndpoint publishEndpoint, DateTimeOffset start, T job,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var jobId = NewId.NextGuid();

        await publishEndpoint.Publish<SubmitJob<T>>(new SubmitJobCommand<T>
        {
            JobId = jobId,
            Job = job,
            Schedule = new RecurringJobScheduleInfo { Start = start }
        }, cancellationToken).ConfigureAwait(false);

        return jobId;
    }

    /// <summary>
    /// Submits a job, returning the accepted jobId
    /// </summary>
    /// <param name="client"></param>
    /// <param name="start">The start time for the job</param>
    /// <param name="job"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<Guid> ScheduleJob<T>(this IRequestClient<SubmitJob<T>> client, DateTimeOffset start, T job,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var jobId = NewId.NextGuid();

        Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new SubmitJobCommand<T>
        {
            JobId = jobId,
            Job = job,
            Schedule = new RecurringJobScheduleInfo { Start = start }
        }, cancellationToken).ConfigureAwait(false);

        return response.Message.JobId;
    }

    /// <summary>
    /// Submits a job, returning the accepted jobId
    /// </summary>
    /// <param name="client"></param>
    /// <param name="start">The start time for the job</param>
    /// <param name="job"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<Guid> ScheduleJob<T>(this IRequestClient<SubmitJob<T>> client, DateTimeOffset start, object job,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var jobId = NewId.NextGuid();

        InitializeContext<T> context = await MessageInitializerCache<T>.Initialize(job, cancellationToken).ConfigureAwait(false);

        Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new SubmitJobCommand<T>
        {
            JobId = jobId,
            Job = context.Message,
            Schedule = new RecurringJobScheduleInfo { Start = start }
        }, cancellationToken).ConfigureAwait(false);

        return response.Message.JobId;
    }

    /// <summary>
    /// Submits a job, returning the accepted jobId
    /// </summary>
    /// <param name="client"></param>
    /// <param name="jobId">Specify an explicit jobId for the job</param>
    /// <param name="start">The start time for the job</param>
    /// <param name="job"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<Guid> ScheduleJob<T>(this IRequestClient<SubmitJob<T>> client, Guid jobId, DateTimeOffset start, T job,
        CancellationToken cancellationToken = default)
        where T : class
    {
        Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new SubmitJobCommand<T>
        {
            JobId = jobId,
            Job = job,
            Schedule = new RecurringJobScheduleInfo { Start = start }
        }, cancellationToken).ConfigureAwait(false);

        return response.Message.JobId;
    }

    /// <summary>
    /// Submits a job, returning the accepted jobId
    /// </summary>
    /// <param name="client"></param>
    /// <param name="jobId">Specify an explicit jobId for the job</param>
    /// <param name="start">The start time for the job</param>
    /// <param name="job"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<Guid> ScheduleJob<T>(this IRequestClient<SubmitJob<T>> client, Guid jobId, DateTimeOffset start, object job,
        CancellationToken cancellationToken = default)
        where T : class
    {
        InitializeContext<T> context = await MessageInitializerCache<T>.Initialize(job, cancellationToken).ConfigureAwait(false);

        Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new SubmitJobCommand<T>
        {
            JobId = jobId,
            Job = context.Message,
            Schedule = new RecurringJobScheduleInfo { Start = start }
        }, cancellationToken).ConfigureAwait(false);

        return response.Message.JobId;
    }
}

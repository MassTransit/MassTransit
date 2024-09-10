namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts.JobService;


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
            Response<JobState> response = await client.GetResponse<JobState>(new { JobId = jobId });

            return response.Message;
        }

        /// <summary>
        /// Submits a job, returning the generated jobId
        /// </summary>
        /// <param name="publishEndpoint"></param>
        /// <param name="job"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<Guid> SubmitJob<T>(this IPublishEndpoint publishEndpoint, T job, CancellationToken cancellationToken = default)
            where T : class
        {
            var jobId = NewId.NextGuid();

            await publishEndpoint.Publish<SubmitJob<T>>(new
            {
                JobId = jobId,
                Job = job
            }, cancellationToken).ConfigureAwait(false);

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
        public static async Task<Guid> SubmitJob<T>(this IRequestClient<SubmitJob<T>> client, T job, CancellationToken cancellationToken = default)
            where T : class
        {
            var jobId = NewId.NextGuid();

            Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = jobId,
                Job = job
            }, cancellationToken).ConfigureAwait(false);

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
        public static async Task<Guid> SubmitJob<T>(this IRequestClient<SubmitJob<T>> client, object job, CancellationToken cancellationToken = default)
            where T : class
        {
            var jobId = NewId.NextGuid();

            Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = jobId,
                Job = job
            }, cancellationToken).ConfigureAwait(false);

            return response.Message.JobId;
        }

        /// <summary>
        /// Submits a job, returning the accepted jobId
        /// </summary>
        /// <param name="client"></param>
        /// <param name="jobId">Specify an explicit jobId for the job</param>
        /// <param name="job"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<Guid> SubmitJob<T>(this IRequestClient<SubmitJob<T>> client, Guid jobId, T job, CancellationToken cancellationToken = default)
            where T : class
        {
            Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = jobId,
                Job = job
            }, cancellationToken).ConfigureAwait(false);

            return response.Message.JobId;
        }

        /// <summary>
        /// Submits a job, returning the accepted jobId
        /// </summary>
        /// <param name="client"></param>
        /// <param name="jobId">Specify an explicit jobId for the job</param>
        /// <param name="job"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<Guid> SubmitJob<T>(this IRequestClient<SubmitJob<T>> client, Guid jobId, object job,
            CancellationToken cancellationToken = default)
            where T : class
        {
            Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = jobId,
                Job = job
            }, cancellationToken).ConfigureAwait(false);

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
    }
}

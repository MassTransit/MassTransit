namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Contracts.JobService;


    public static class JobServiceExtensions
    {
        public static async Task<JobState> GetJobState(this IRequestClient<GetJobState> client, Guid jobId)
        {
            Response<JobState> response = await client.GetResponse<JobState>(new { JobId = jobId });

            return response.Message;
        }
    }
}

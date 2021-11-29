#nullable enable
namespace MassTransit
{
    using Contracts.JobService;


    public static class JobServiceEventExtensions
    {
        /// <summary>
        /// Returns the job from the message
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static TJob? GetJob<TJob>(this ConsumeContext<StartJob> context)
            where TJob : class
        {
            return context.SerializerContext.DeserializeObject<TJob>(context.Message.Job);
        }

        public static TJob? GetJob<TJob>(this ConsumeContext<FaultJob> context)
            where TJob : class
        {
            return context.SerializerContext.DeserializeObject<TJob>(context.Message.Job);
        }

        public static TJob? GetJob<TJob>(this ConsumeContext<CompleteJob> context)
            where TJob : class
        {
            return context.SerializerContext.DeserializeObject<TJob>(context.Message.Job);
        }

        public static TJob? GetJob<TJob>(this ConsumeContext<JobCompleted> context)
            where TJob : class
        {
            return context.SerializerContext.DeserializeObject<TJob>(context.Message.Job);
        }

        public static TResult? GetResult<TResult>(this ConsumeContext<JobCompleted> context)
            where TResult : class
        {
            return context.SerializerContext.DeserializeObject<TResult>(context.Message.Result);
        }

        public static TJob? GetJob<TJob>(this ConsumeContext<JobFaulted> context)
            where TJob : class
        {
            return context.SerializerContext.DeserializeObject<TJob>(context.Message.Job);
        }
    }
}

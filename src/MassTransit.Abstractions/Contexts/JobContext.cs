namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    public interface JobContext :
        PipeContext,
        MessageContext,
        ISendEndpointProvider,
        IPublishEndpoint
    {
        Guid JobId { get; }
        Guid AttemptId { get; }
        int RetryAttempt { get; }

        TimeSpan ElapsedTime { get; }

        Task NotifyCanceled(string? reason = null);
        Task NotifyStarted();
        Task NotifyCompleted();
        Task NotifyFaulted(Exception exception, TimeSpan? delay = default);
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
}

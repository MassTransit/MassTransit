namespace MassTransit.HangfireIntegration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Hangfire;
    using Scheduling;


    public class ScheduleMessageConsumer :
        IConsumer<ScheduleMessage>,
        IConsumer<CancelScheduledMessage>
    {
        readonly IBackgroundJobClient _backgroundJobClient;

        public ScheduleMessageConsumer(IHangfireComponentResolver hangfireComponentResolver)
            : this(hangfireComponentResolver.BackgroundJobClient)
        {
        }

        ScheduleMessageConsumer(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task Consume(ConsumeContext<ScheduleMessage> context)
        {
            var message = HangfireSerializedMessage.Create(context);
            var jobId = _backgroundJobClient.Schedule<ScheduleJob>(
                x => x.SendMessage(message, CancellationToken.None),
                context.Message.ScheduledTime);

            LogContext.Debug?.Log("Scheduled: {Id}", jobId);
        }

        public async Task Consume(ConsumeContext<CancelScheduledMessage> context)
        {
            var jobId = context.Message.TokenId.ToString("N");
            var deletedJob = _backgroundJobClient.Delete(jobId);
            if (deletedJob)
                LogContext.Debug?.Log("Cancelled Scheduled Message: {Id} at {Timestamp}", jobId, context.Message.Timestamp);
            else
                LogContext.Debug?.Log("CancelScheduledMessage: no message found for {Id}", jobId);
        }
    }
}

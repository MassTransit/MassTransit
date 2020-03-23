namespace MassTransit.HangfireIntegration
{
    using System.Threading.Tasks;
    using Hangfire;
    using Scheduling;


    public class ScheduleMessageConsumer :
        IConsumer<ScheduleMessage>
    {
        readonly IBackgroundJobClient _backgroundJobClient;

        public ScheduleMessageConsumer(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task Consume(ConsumeContext<ScheduleMessage> context)
        {
            var message = HangfireSerializedMessage.Create(context);
            _backgroundJobClient.Schedule<ScheduleJob>(
                x => x.SendMessage(message).ConfigureAwait(false),
                context.Message.ScheduledTime);
        }
    }
}

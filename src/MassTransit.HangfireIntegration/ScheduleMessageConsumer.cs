namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Threading.Tasks;
    using Hangfire;
    using Scheduling;


    public class ScheduleMessageConsumer :
        IConsumer<ScheduleMessage>
    {
        readonly Func<IBackgroundJobClient> _backgroundJobClient;

        public ScheduleMessageConsumer(Func<IBackgroundJobClient> backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task Consume(ConsumeContext<ScheduleMessage> context)
        {
            var message = HangfireSerializedMessage.Create(context);
            _backgroundJobClient().Schedule<ScheduleJob>(x => x.SendMessage(message), context.Message.ScheduledTime);
        }
    }
}

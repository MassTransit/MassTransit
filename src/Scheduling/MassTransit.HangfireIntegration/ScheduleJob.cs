namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using GreenPipes;
    using Hangfire;
    using Hangfire.Server;
    using Serialization;


    [Queue(SchedulerBusObserver.DefaultQueueName)]
    class ScheduleJob
    {
        readonly IBus _bus;

        public ScheduleJob(IBus bus)
        {
            _bus = bus;
        }

        [SchedulerJobData]
        public async Task SendMessage(HangfireScheduledMessageData messageData, PerformContext performContext)
        {
            try
            {
                IPipe<SendContext> sendPipe = CreateMessageContext(messageData, _bus.Address);

                var endpoint = await _bus.GetSendEndpoint(messageData.Destination).ConfigureAwait(false);

                var scheduled = new Scheduled();

                await endpoint.Send(scheduled, sendPipe, performContext.CancellationToken.ShutdownToken).ConfigureAwait(false);

                LogContext.Debug?.Log("Schedule Executed: {JobId}, created at: {CreatedAt}", performContext.BackgroundJob.Id,
                    performContext.BackgroundJob.CreatedAt);
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Failed to send scheduled message: {JobId}, created at: {CreatedAt}, destination: {DestinationAddress}",
                    performContext.BackgroundJob.Id, messageData.Destination, performContext.BackgroundJob.CreatedAt);

                throw new JobPerformanceException("Job Execution exception", ex);
            }
        }

        [SchedulerJobData]
        [RecurringScheduleDateTimeInterval]
        public async Task SendMessage(HangfireRecurringScheduledMessageData messageData, PerformContext performContext)
        {
            try
            {
                IPipe<SendContext> sendPipe = CreateMessageContext(messageData, _bus.Address, messageData.JobKey);

                var endpoint = await _bus.GetSendEndpoint(messageData.Destination).ConfigureAwait(false);

                var scheduled = new Scheduled();

                await endpoint.Send(scheduled, sendPipe, performContext.CancellationToken.ShutdownToken).ConfigureAwait(false);

                LogContext.Debug?.Log("Schedule Executed: {JobId}, created at: {CreatedAt}, with range: {StartTime}-{EndTime}", performContext.BackgroundJob.Id,
                    performContext.BackgroundJob.CreatedAt, messageData.StartTime, messageData.EndTime);
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Failed to send scheduled message: {JobId}, created at: {CreatedAt}, destination: {DestinationAddress}",
                    performContext.BackgroundJob.Id, messageData.Destination, performContext.BackgroundJob.CreatedAt);

                throw new JobPerformanceException("Job Execution exception", ex);
            }
        }

        static IPipe<SendContext> CreateMessageContext(SerializedMessage message, Uri sourceAddress, string jobKey = null)
        {
            IPipe<SendContext> sendPipe = Pipe.Execute<SendContext>(context =>
            {
                if (!string.IsNullOrEmpty(jobKey))
                    context.Headers.Set(HangfireMessageHeaders.TriggerKey, jobKey);
            });

            return new SerializedMessageContextAdapter(sendPipe, message, sourceAddress);
        }


        class Scheduled
        {
        }
    }
}

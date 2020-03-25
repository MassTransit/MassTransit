namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Hangfire.Server;
    using Serialization;


    class ScheduleJob
    {
        readonly IBus _bus;

        public ScheduleJob(IBus bus)
        {
            _bus = bus;
        }

        public async Task SendMessage(HangfireScheduledMessageData messageData, PerformContext performContext)
        {
            try
            {
                IPipe<SendContext> sendPipe = CreateMessageContext(messageData, _bus.Address, performContext);

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

        [RecurringScheduleDateTimeInterval]
        public async Task SendMessage(HangfireRecurringScheduledMessageData messageData, PerformContext performContext)
        {
            try
            {
                IPipe<SendContext> sendPipe = CreateMessageContext(messageData, _bus.Address, performContext, messageData.JobKey);

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

        static IPipe<SendContext> CreateMessageContext(SerializedMessage message, Uri sourceAddress, PerformContext performContext, string jobKey = null)
        {
            IPipe<SendContext> sendPipe = Pipe.New<SendContext>(x =>
            {
                x.UseExecute(context =>
                {
                    if (!string.IsNullOrEmpty(jobKey))
                        context.Headers.Set(MessageHeaders.QuartzTriggerKey, jobKey);
                    context.Headers.Set(MessageHeaders.Quartz.Scheduled, performContext.BackgroundJob.CreatedAt.ToString("O"));
                    context.Headers.Set(MessageHeaders.Quartz.Sent, ((DateTime)InVar.Timestamp).ToString("O"));
                });
            });

            return new SerializedMessageContextAdapter(sendPipe, message, sourceAddress);
        }


        class Scheduled
        {
        }
    }
}

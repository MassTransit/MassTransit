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

        public async Task SendMessage(HangfireSerializedMessage message, PerformContext performContext)
        {
            try
            {
                IPipe<SendContext> sendPipe = CreateMessageContext(message, _bus.Address, message.JobKey);

                var endpoint = await _bus.GetSendEndpoint(message.Destination).ConfigureAwait(false);

                var scheduled = new Scheduled();

                await endpoint.Send(scheduled, sendPipe, performContext.CancellationToken.ShutdownToken).ConfigureAwait(false);

                LogContext.Debug?.Log("Schedule Executed: {JobId}, created at: {CreatedAt}", performContext.BackgroundJob.Id,
                    performContext.BackgroundJob.CreatedAt);
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Failed to send scheduled message: {JobId}, created at: {CreatedAt}, destination: {DestinationAddress}",
                    performContext.BackgroundJob.Id, message.Destination, performContext.BackgroundJob.CreatedAt);

                throw new JobPerformanceException("Job Execution exception", ex);
            }
        }

        [DateIntervalAttribute]
        public async Task SendMessage(HangfireSerializedMessage message, DateTimeOffset startDate, DateTimeOffset? endDate, PerformContext performContext)
        {
            try
            {
                IPipe<SendContext> sendPipe = CreateMessageContext(message, _bus.Address, message.JobKey);

                var endpoint = await _bus.GetSendEndpoint(message.Destination).ConfigureAwait(false);

                var scheduled = new Scheduled();

                await endpoint.Send(scheduled, sendPipe, performContext.CancellationToken.ShutdownToken).ConfigureAwait(false);

                LogContext.Debug?.Log("Schedule Executed: {JobId}, created at: {CreatedAt}, with range: {StartDate}-{EndDate}", performContext.BackgroundJob.Id,
                    performContext.BackgroundJob.CreatedAt, startDate, endDate);
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Failed to send scheduled message: {JobId}, created at: {CreatedAt}, destination: {DestinationAddress}",
                    performContext.BackgroundJob.Id, message.Destination, performContext.BackgroundJob.CreatedAt);

                throw new JobPerformanceException("Job Execution exception", ex);
            }
        }

        static IPipe<SendContext> CreateMessageContext(SerializedMessage message, Uri sourceAddress, string jobKey)
        {
            IPipe<SendContext> sendPipe = Pipe.New<SendContext>(x =>
            {
                x.UseExecute(context =>
                {
                    if (!string.IsNullOrEmpty(jobKey))
                        context.Headers.Set(MessageHeaders.QuartzTriggerKey, jobKey);
                });
            });

            return new SerializedMessageContextAdapter(sendPipe, message, sourceAddress);
        }


        class Scheduled
        {
        }
    }
}

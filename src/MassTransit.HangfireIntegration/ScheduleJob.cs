namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Threading;
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

        public async Task SendMessage(HangfireSerializedMessage message, CancellationToken cancellationToken)
        {
            try
            {
                IPipe<SendContext> sendPipe = CreateMessageContext(message, _bus.Address, message.JobKey);

                var endpoint = await _bus.GetSendEndpoint(message.Destination).ConfigureAwait(false);

                var scheduled = new Scheduled();

                await endpoint.Send(scheduled, sendPipe, cancellationToken).ConfigureAwait(false);

                LogContext.Debug?.Log("Schedule Executed: {CorrelationId}", message.CorrelationId);
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Failed to send scheduled message: {CorrelationId}, destination: {DestinationAddress}",
                    message.CorrelationId, message.Destination);

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

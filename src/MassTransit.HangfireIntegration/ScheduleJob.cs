namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Threading.Tasks;
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

        public async Task SendMessage(HangfireSerializedMessage serializedMessage)
        {
            try
            {
                IPipe<SendContext> sendPipe = CreateMessageContext(serializedMessage, _bus.Address);

                var endpoint = await _bus.GetSendEndpoint(serializedMessage.Destination).ConfigureAwait(false);

                var scheduled = new Scheduled();

                await endpoint.Send(scheduled, sendPipe).ConfigureAwait(false);

                //LogContext.Debug?.Log("Schedule Executed: {Key} {Schedule}", context.JobDetail.Key, context.Trigger.GetNextFireTimeUtc());
            }
            catch (Exception ex)
            {
                //LogContext.Error?.Log(ex, "Failed to send scheduled message, type: {MessageType}, destination: {DestinationAddress}", MessageType, Destination);

                throw new JobPerformanceException("Job Execution exception", ex);
            }
        }

        static IPipe<SendContext> CreateMessageContext(HangfireSerializedMessage serializedMessage, Uri sourceAddress)
        {
            IPipe<SendContext> sendPipe = Pipe.New<SendContext>(x =>
            {
                x.UseExecute(context =>
                {
                    if (!string.IsNullOrEmpty(serializedMessage.JobKey))
                        context.Headers.Set(MessageHeaders.QuartzTriggerKey, serializedMessage.JobKey);
                });
            });

            return new SerializedMessageContextAdapter(sendPipe, serializedMessage, sourceAddress);
        }


        class Scheduled
        {
        }
    }
}

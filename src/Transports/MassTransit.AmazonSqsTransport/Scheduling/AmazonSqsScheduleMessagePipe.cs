namespace MassTransit.AmazonSqsTransport.Scheduling
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using MassTransit.Scheduling;


    /// <summary>
    /// Sets the message enqueue time when sending the message, and invokes
    /// any developer-specified pipes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AmazonSqsScheduleMessagePipe<T> :
        ScheduleMessageContextPipe<T>
        where T : class
    {
        readonly DateTime _scheduledTime;

        public AmazonSqsScheduleMessagePipe(DateTime scheduledTime, IPipe<SendContext<T>> pipe)
            : base(pipe)
        {
            _scheduledTime = scheduledTime;
        }

        public override Task Send(SendContext<T> context)
        {
            ScheduledMessageId = ScheduleTokenIdCache<T>.GetTokenId(context.Message, context.MessageId);

            var sqsSendContext = context.GetPayload<AmazonSqsSendContext>();

            var delay = Math.Max(0, (_scheduledTime.Kind == DateTimeKind.Local
                ? _scheduledTime - DateTime.Now
                : _scheduledTime - DateTime.UtcNow).TotalSeconds);

            if (delay > 0)
                sqsSendContext.DelaySeconds = (int)delay;

            return base.Send(context);
        }
    }
}

namespace MassTransit.Azure.ServiceBus.Core.Scheduling
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Scheduling;


    /// <summary>
    /// Sets the message enqueue time when sending the message, and invokes
    /// any developer-specified pipes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceBusScheduleMessagePipe<T> :
        ScheduleMessageContextPipe<T>
        where T : class
    {
        readonly DateTime _scheduledTime;

        public ServiceBusScheduleMessagePipe(DateTime scheduledTime)
        {
            _scheduledTime = scheduledTime;
        }

        public ServiceBusScheduleMessagePipe(DateTime scheduledTime, IPipe<SendContext<T>> pipe)
            : base(pipe)
        {
            _scheduledTime = scheduledTime;
        }

        public override Task Send(SendContext<T> context)
        {
            context.SetScheduledEnqueueTime(_scheduledTime);

            return base.Send(context);
        }
    }


    /// <summary>
    /// Sets the message enqueue time when sending the message, and invokes
    /// any developer-specified pipes.
    /// </summary>
    public class ServiceBusScheduleMessagePipe :
        ScheduleMessageContextPipe
    {
        readonly DateTime _scheduledTime;

        public ServiceBusScheduleMessagePipe(DateTime scheduledTime)
        {
            _scheduledTime = scheduledTime;
        }

        public ServiceBusScheduleMessagePipe(DateTime scheduledTime, IPipe<SendContext> pipe)
            : base(pipe)
        {
            _scheduledTime = scheduledTime;
        }

        public override Task Send(SendContext context)
        {
            context.SetScheduledEnqueueTime(_scheduledTime);

            return base.Send(context);
        }
    }
}

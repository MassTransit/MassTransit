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
    public class ServiceBusScheduleSendPipe<T> :
        ScheduleSendPipe<T>
        where T : class
    {
        readonly DateTime _scheduledTime;

        public ServiceBusScheduleSendPipe(IPipe<SendContext<T>> pipe, DateTime scheduledTime)
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
}

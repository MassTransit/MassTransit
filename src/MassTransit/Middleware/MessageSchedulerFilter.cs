namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Scheduling;


    /// <summary>
    /// Adds the scheduler to the consume context, so that it can be used for message redelivery
    /// </summary>
    public class MessageSchedulerFilter :
        IFilter<ConsumeContext>
    {
        readonly Uri _schedulerAddress;

        public MessageSchedulerFilter(Uri schedulerAddress)
        {
            _schedulerAddress = schedulerAddress;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("scheduler");
            scope.Add("type", "send");
            scope.Add("address", _schedulerAddress);
        }

        public Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            context.GetOrAddPayload<MessageSchedulerContext>(() => new ConsumeMessageSchedulerContext(context, SchedulerFactory));

            return next.Send(context);
        }

        IMessageScheduler SchedulerFactory(ConsumeContext context)
        {
            return new MessageScheduler(new EndpointScheduleMessageProvider(() => context.GetSendEndpoint(_schedulerAddress)),
                context.GetPayload<IBusTopology>());
        }
    }
}

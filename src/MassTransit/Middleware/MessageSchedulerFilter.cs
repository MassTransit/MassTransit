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

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("scheduler");
            scope.Add("type", "send");
            scope.Add("address", _schedulerAddress);
        }

        Task IFilter<ConsumeContext>.Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            context.GetOrAddPayload<MessageSchedulerContext>(() => new ConsumeMessageSchedulerContext(context,
                x => new MessageScheduler(new EndpointScheduleMessageProvider(() => x.GetSendEndpoint(_schedulerAddress)), x.GetPayload<IBusTopology>()),
                context.ReceiveContext.InputAddress));

            return next.Send(context);
        }
    }
}

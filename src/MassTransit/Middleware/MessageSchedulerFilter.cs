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
            MessageSchedulerContext PayloadFactory()
            {
                IMessageScheduler Factory()
                {
                    return new MessageScheduler(new EndpointScheduleMessageProvider(() => context.GetSendEndpoint(_schedulerAddress)),
                        context.GetPayload<IBusTopology>());
                }

                return new ConsumeMessageSchedulerContext(Factory, context.ReceiveContext.InputAddress);
            }

            context.GetOrAddPayload(PayloadFactory);

            return next.Send(context);
        }
    }
}

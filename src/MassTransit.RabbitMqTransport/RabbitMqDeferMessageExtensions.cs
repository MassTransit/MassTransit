namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using RabbitMqTransport;
    using RabbitMqTransport.Contexts;
    using RabbitMqTransport.Scheduling;
    using Scheduling;


    public static class RabbitMqDeferMessageExtensions
    {
        /// <summary>
        /// Defers the message for redelivery using a delayed exchange (an experimental RabbitMQ plug-in).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="delay"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Task Defer<T>(this ConsumeContext<T> context, TimeSpan delay, Action<ConsumeContext, SendContext> callback = null)
            where T : class
        {
            if (!context.TryGetPayload(out IMessageScheduler scheduler))
            {
                if (!context.TryGetPayload(out ModelContext modelContext))
                    throw new ArgumentException("A valid message scheduler was not found, and no ModelContext was available", nameof(context));

                var provider = new DelayedExchangeScheduleMessageProvider(context, modelContext.ConnectionContext.Topology);

                scheduler = new MessageScheduler(provider);
            }

            MessageRedeliveryContext redeliveryContext = new DelayedExchangeMessageRedeliveryContext<T>(context, scheduler);

            return redeliveryContext.ScheduleRedelivery(delay, callback);
        }
    }
}

namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Middleware;


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
            MessageRedeliveryContext redeliveryContext = new DelayedMessageRedeliveryContext<T>(context, RedeliveryOptions.None);

            return redeliveryContext.ScheduleRedelivery(delay, callback);
        }
    }
}

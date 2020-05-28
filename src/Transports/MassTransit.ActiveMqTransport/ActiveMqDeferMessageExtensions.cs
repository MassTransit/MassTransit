namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Threading.Tasks;
    using Contexts;


    public static class ActiveMqDeferMessageExtensions
    {
        /// <summary>
        /// Defers the message for redelivery using a delayed exchange.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="delay"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Task Defer<T>(this ConsumeContext<T> context, TimeSpan delay, Action<ConsumeContext, SendContext> callback = null)
            where T : class
        {
            MessageRedeliveryContext redeliveryContext = new ActiveMqMessageRedeliveryContext<T>(context);

            return redeliveryContext.ScheduleRedelivery(delay, callback);
        }
    }
}

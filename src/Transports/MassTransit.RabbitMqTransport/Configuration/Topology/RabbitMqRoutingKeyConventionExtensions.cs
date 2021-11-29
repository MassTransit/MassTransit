namespace MassTransit
{
    using System;
    using RabbitMqTransport;
    using RabbitMqTransport.Configuration;


    public static class RabbitMqRoutingKeyConventionExtensions
    {
        public static void UseRoutingKeyFormatter<T>(this IMessageSendTopologyConfigurator<T> configurator, IMessageRoutingKeyFormatter<T> formatter)
            where T : class
        {
            configurator.UpdateConvention<IRoutingKeyMessageSendTopologyConvention<T>>(
                update =>
                {
                    update.SetFormatter(formatter);

                    return update;
                });
        }

        /// <summary>
        /// Use the routing key formatter for the specified message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="formatter"></param>
        public static void UseRoutingKeyFormatter<T>(this ISendTopologyConfigurator configurator, IMessageRoutingKeyFormatter<T> formatter)
            where T : class
        {
            configurator.GetMessageTopology<T>().UseRoutingKeyFormatter(formatter);
        }

        /// <summary>
        /// Use the delegate to format the routing key, using Empty if the string is null upon return
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="formatter"></param>
        public static void UseRoutingKeyFormatter<T>(this ISendTopologyConfigurator configurator, Func<SendContext<T>, string> formatter)
            where T : class
        {
            configurator.GetMessageTopology<T>().UseRoutingKeyFormatter(new DelegateRoutingKeyFormatter<T>(formatter));
        }

        /// <summary>
        /// Use the delegate to format the routing key, using Empty if the string is null upon return
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="formatter"></param>
        public static void UseRoutingKeyFormatter<T>(this IMessageSendTopologyConfigurator<T> configurator, Func<SendContext<T>, string> formatter)
            where T : class
        {
            configurator.UseRoutingKeyFormatter(new DelegateRoutingKeyFormatter<T>(formatter));
        }
    }
}

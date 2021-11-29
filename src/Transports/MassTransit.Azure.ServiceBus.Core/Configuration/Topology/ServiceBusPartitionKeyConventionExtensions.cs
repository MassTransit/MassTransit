namespace MassTransit
{
    using System;
    using AzureServiceBusTransport;
    using AzureServiceBusTransport.Configuration;


    public static class ServiceBusPartitionKeyConventionExtensions
    {
        public static void UsePartitionKeyFormatter<T>(this IMessageSendTopologyConfigurator<T> configurator, IMessagePartitionKeyFormatter<T> formatter)
            where T : class
        {
            configurator.UpdateConvention<IPartitionKeyMessageSendTopologyConvention<T>>(
                update =>
                {
                    update.SetFormatter(formatter);

                    return update;
                });
        }

        /// <summary>
        /// Use the partition key formatter for the specified message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="formatter"></param>
        public static void UsePartitionKeyFormatter<T>(this ISendTopologyConfigurator configurator, IMessagePartitionKeyFormatter<T> formatter)
            where T : class
        {
            configurator.GetMessageTopology<T>().UsePartitionKeyFormatter(formatter);
        }

        /// <summary>
        /// Use the delegate to format the partition key, using Empty if the string is null upon return
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="formatter"></param>
        public static void UsePartitionKeyFormatter<T>(this ISendTopologyConfigurator configurator, Func<SendContext<T>, string> formatter)
            where T : class
        {
            configurator.GetMessageTopology<T>().UsePartitionKeyFormatter(new DelegatePartitionKeyFormatter<T>(formatter));
        }

        /// <summary>
        /// Use the delegate to format the partition key, using Empty if the string is null upon return
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="formatter"></param>
        public static void UsePartitionKeyFormatter<T>(this IMessageSendTopologyConfigurator<T> configurator, Func<SendContext<T>, string> formatter)
            where T : class
        {
            configurator.UsePartitionKeyFormatter(new DelegatePartitionKeyFormatter<T>(formatter));
        }
    }
}

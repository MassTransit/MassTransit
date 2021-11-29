namespace MassTransit
{
    using System;
    using AzureServiceBusTransport;
    using AzureServiceBusTransport.Configuration;


    public static class ServiceBusSessionIdConventionExtensions
    {
        public static void UseSessionIdFormatter<T>(this IMessageSendTopologyConfigurator<T> configurator, IMessageSessionIdFormatter<T> formatter)
            where T : class
        {
            configurator.UpdateConvention<ISessionIdMessageSendTopologyConvention<T>>(
                update =>
                {
                    update.SetFormatter(formatter);

                    return update;
                });
        }

        /// <summary>
        /// Use the session id formatter for the specified message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="formatter"></param>
        public static void UseSessionIdFormatter<T>(this ISendTopologyConfigurator configurator, IMessageSessionIdFormatter<T> formatter)
            where T : class
        {
            configurator.GetMessageTopology<T>().UseSessionIdFormatter(formatter);
        }

        /// <summary>
        /// Use the delegate to format the session id, using Empty if the string is null upon return
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="formatter"></param>
        public static void UseSessionIdFormatter<T>(this ISendTopologyConfigurator configurator, Func<SendContext<T>, string> formatter)
            where T : class
        {
            configurator.GetMessageTopology<T>().UseSessionIdFormatter(new DelegateSessionIdFormatter<T>(formatter));
        }

        /// <summary>
        /// Use the delegate to format the session id, using Empty if the string is null upon return
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="formatter"></param>
        public static void UseSessionIdFormatter<T>(this IMessageSendTopologyConfigurator<T> configurator, Func<SendContext<T>, string> formatter)
            where T : class
        {
            configurator.UseSessionIdFormatter(new DelegateSessionIdFormatter<T>(formatter));
        }
    }
}

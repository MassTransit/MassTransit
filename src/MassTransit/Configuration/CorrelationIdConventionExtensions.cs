namespace MassTransit
{
    using System;
    using Configuration;
    using Topology;


    public static class CorrelationIdConventionExtensions
    {
        /// <summary>
        /// Specify for the message type that the delegate be used for setting the CorrelationId
        /// property of the message envelope.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="correlationIdSelector"></param>
        public static void UseCorrelationId<T>(this IMessageSendTopologyConfigurator<T> configurator, Func<T, Guid> correlationIdSelector)
            where T : class
        {
            configurator.AddOrUpdateConvention<ICorrelationIdMessageSendTopologyConvention<T>>(
                () =>
                {
                    var convention = new CorrelationIdMessageSendTopologyConvention<T>();
                    convention.SetCorrelationId(new DelegateMessageCorrelationId<T>(correlationIdSelector));

                    return convention;
                },
                update =>
                {
                    update.SetCorrelationId(new DelegateMessageCorrelationId<T>(correlationIdSelector));

                    return update;
                });
        }

        /// <summary>
        /// Specify for the message type that the delegate be used for setting the CorrelationId
        /// property of the message envelope.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="correlationIdSelector"></param>
        public static void UseCorrelationId<T>(this IMessageSendTopologyConfigurator<T> configurator, Func<T, Guid?> correlationIdSelector)
            where T : class
        {
            configurator.AddOrUpdateConvention<ICorrelationIdMessageSendTopologyConvention<T>>(
                () =>
                {
                    var convention = new CorrelationIdMessageSendTopologyConvention<T>();
                    convention.SetCorrelationId(new NullableDelegateMessageCorrelationId<T>(correlationIdSelector));

                    return convention;
                },
                update =>
                {
                    update.SetCorrelationId(new NullableDelegateMessageCorrelationId<T>(correlationIdSelector));

                    return update;
                });
        }

        /// <summary>
        /// Specify for the message type that the delegate be used for setting the CorrelationId
        /// property of the message envelope.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="correlationIdSelector"></param>
        public static void UseCorrelationId<T>(this ISendTopology configurator, Func<T, Guid> correlationIdSelector)
            where T : class
        {
            configurator.GetMessageTopology<T>().UseCorrelationId(correlationIdSelector);
        }

        /// <summary>
        /// Specify for the message type that the delegate be used for setting the CorrelationId
        /// property of the message envelope.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="correlationIdSelector"></param>
        public static void UseCorrelationId<T>(this ISendTopology configurator, Func<T, Guid?> correlationIdSelector)
            where T : class
        {
            configurator.GetMessageTopology<T>().UseCorrelationId(correlationIdSelector);
        }
    }
}

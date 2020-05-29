namespace MassTransit
{
    using System;
    using Context.SetCorrelationIds;
    using Topology;
    using Topology.Conventions;
    using Topology.Conventions.CorrelationId;


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
                    convention.SetCorrelationId(new DelegateSetCorrelationId<T>(correlationIdSelector));

                    return convention;
                },
                update =>
                {
                    update.SetCorrelationId(new DelegateSetCorrelationId<T>(correlationIdSelector));

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
                    convention.SetCorrelationId(new NullableDelegateSetCorrelationId<T>(correlationIdSelector));

                    return convention;
                },
                update =>
                {
                    update.SetCorrelationId(new NullableDelegateSetCorrelationId<T>(correlationIdSelector));

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

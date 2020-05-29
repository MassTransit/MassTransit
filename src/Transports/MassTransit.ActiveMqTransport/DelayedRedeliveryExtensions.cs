namespace MassTransit.ActiveMqTransport
{
    using System;
    using Configurators;
    using GreenPipes;
    using GreenPipes.Configurators;
    using PipeConfigurators;
    using Specifications;


    public static class DelayedRedeliveryExtensions
    {
        /// <summary>
        /// Use the message scheduler to schedule redelivery of a specific message type based upon the retry policy, via
        /// the delayed exchange feature of ActiveMQ.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void UseDelayedRedelivery<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, Action<IRetryConfigurator> configure)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var redeliverySpecification = new ActiveMqRedeliveryPipeSpecification<T>();

            configurator.AddPipeSpecification(redeliverySpecification);

            var retrySpecification = new RedeliveryRetryPipeSpecification<T>();

            configure?.Invoke(retrySpecification);

            configurator.AddPipeSpecification(retrySpecification);
        }

        /// <summary>
        /// For all configured messages type (handlers, consumers, and sagas), configures delayed redelivery using the retry configuration specified.
        /// Redelivery is configured once for each message type, and is added prior to the consumer factory or saga repository in the pipeline.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configureRetry"></param>
        public static void UseDelayedRedelivery(this IActiveMqReceiveEndpointConfigurator configurator, Action<IRetryConfigurator> configureRetry)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (configureRetry == null)
                throw new ArgumentNullException(nameof(configureRetry));

            var observer = new ActiveMqRedeliveryConfigurationObserver(configurator, configureRetry);
        }
    }
}

namespace MassTransit
{
    using System;
    using Configuration;


    /// <summary>
    /// A consumer definition defines the configuration for a consumer, which can be used by the automatic registration code to
    /// configure the consumer on a receive endpoint.
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    public class ConsumerDefinition<TConsumer> :
        IConsumerDefinition<TConsumer>
        where TConsumer : class, IConsumer
    {
        int? _concurrentMessageLimit;
        string? _endpointName;

        protected ConsumerDefinition()
        {
            // TODO if the partitionKey is specified, use a partition filter instead of a semaphore
        }

        /// <summary>
        /// Specify the endpoint name (which may be a queue, or a subscription, depending upon the transport) on which the consumer
        /// should be configured.
        /// </summary>
        protected string EndpointName
        {
            set => _endpointName = value;
        }

        public IEndpointDefinition<TConsumer>? EndpointDefinition { get; set; }

        IEndpointDefinition? IConsumerDefinition.EndpointDefinition => EndpointDefinition;

        /// Set the concurrent message limit for the consumer, which limits how many consumers are able to concurrently
        /// consume messages.
        public int? ConcurrentMessageLimit
        {
            get => _concurrentMessageLimit;
            protected set => _concurrentMessageLimit = value;
        }

        void IConsumerDefinition<TConsumer>.Configure(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TConsumer> consumerConfigurator)
        {
            if (_concurrentMessageLimit.HasValue)
                consumerConfigurator.ConcurrentMessageLimit = _concurrentMessageLimit;

            ConfigureConsumer(endpointConfigurator, consumerConfigurator);
        }

        Type IConsumerDefinition.ConsumerType => typeof(TConsumer);

        string IConsumerDefinition.GetEndpointName(IEndpointNameFormatter formatter)
        {
            return string.IsNullOrWhiteSpace(_endpointName)
                ? _endpointName = EndpointDefinition?.GetEndpointName(formatter) ?? formatter.Consumer<TConsumer>()
                : _endpointName!;
        }

        /// <summary>
        /// Configure the consumer endpoint
        /// </summary>
        /// <param name="configure"></param>
        protected void Endpoint(Action<IEndpointRegistrationConfigurator>? configure = null)
        {
            var configurator = new EndpointRegistrationConfigurator<TConsumer>();

            configure?.Invoke(configurator);

            EndpointDefinition = new ConsumerEndpointDefinition<TConsumer>(configurator.Settings);
        }

        /// <summary>
        /// Called when the consumer is being configured on the endpoint. Configuration only applies to this consumer, and does not apply to
        /// the endpoint.
        /// </summary>
        /// <param name="endpointConfigurator">The receive endpoint configurator for the consumer</param>
        /// <param name="consumerConfigurator">The consumer configurator</param>
        protected virtual void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TConsumer> consumerConfigurator)
        {
        }
    }
}

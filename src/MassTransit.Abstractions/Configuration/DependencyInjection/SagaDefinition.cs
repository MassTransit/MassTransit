namespace MassTransit
{
    using System;
    using Configuration;


    /// <summary>
    /// A saga definition defines the configuration for a saga, which can be used by the automatic registration code to
    /// configure the consumer on a receive endpoint.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class SagaDefinition<TSaga> :
        ISagaDefinition<TSaga>
        where TSaga : class, ISaga
    {
        int? _concurrentMessageLimit;
        string? _endpointName;

        protected SagaDefinition()
        {
        }

        /// <summary>
        /// Specify the endpoint name (which may be a queue, or a subscription, depending upon the transport) on which the saga
        /// should be configured.
        /// </summary>
        protected string EndpointName
        {
            set => _endpointName = value;
        }

        public IEndpointDefinition<TSaga>? EndpointDefinition { get; set; }

        IEndpointDefinition? ISagaDefinition.EndpointDefinition => EndpointDefinition;

        /// <summary>
        /// Set the concurrent message limit for the saga, which limits how many saga instances are able to concurrently
        /// consume messages.
        /// </summary>
        public int? ConcurrentMessageLimit
        {
            get => _concurrentMessageLimit;
            protected set => _concurrentMessageLimit = value;
        }

        void ISagaDefinition<TSaga>.Configure(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<TSaga> sagaConfigurator)
        {
            if (_concurrentMessageLimit.HasValue)
                sagaConfigurator.ConcurrentMessageLimit = _concurrentMessageLimit;

            ConfigureSaga(endpointConfigurator, sagaConfigurator);
        }

        Type ISagaDefinition.SagaType => typeof(TSaga);

        string ISagaDefinition.GetEndpointName(IEndpointNameFormatter formatter)
        {
            return string.IsNullOrWhiteSpace(_endpointName)
                ? _endpointName = EndpointDefinition?.GetEndpointName(formatter) ?? formatter.Saga<TSaga>()
                : _endpointName!;
        }

        /// <summary>
        /// Called when configuring the saga on the endpoint. Configuration only applies to this saga, and does not apply to
        /// the endpoint.
        /// </summary>
        /// <param name="endpointConfigurator">The receive endpoint configurator for the consumer</param>
        /// <param name="sagaConfigurator">The saga configurator</param>
        protected virtual void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<TSaga> sagaConfigurator)
        {
        }

        /// <summary>
        /// Configure the saga endpoint
        /// </summary>
        /// <param name="configure"></param>
        protected void Endpoint(Action<IEndpointRegistrationConfigurator>? configure = null)
        {
            var configurator = new EndpointRegistrationConfigurator<TSaga>();

            configure?.Invoke(configurator);

            EndpointDefinition = new SagaEndpointDefinition<TSaga>(configurator.Settings);
        }
    }
}

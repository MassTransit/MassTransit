namespace MassTransit.Registration
{
    using System;
    using Automatonymous;
    using Futures;


    /// <summary>
    /// A future definition defines the configuration for a future, which can be used by the automatic registration code to
    /// configure the consumer on a receive endpoint.
    /// </summary>
    /// <typeparam name="TFuture"></typeparam>
    public class FutureDefinition<TFuture> :
        IFutureDefinition<TFuture>
        where TFuture : MassTransitStateMachine<FutureState>
    {
        int? _concurrentMessageLimit;
        string _endpointName;

        protected FutureDefinition()
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

        public IEndpointDefinition<TFuture> EndpointDefinition { get; set; }

        IEndpointDefinition IFutureDefinition.EndpointDefinition => EndpointDefinition;

        /// <summary>
        /// Set the concurrent message limit for the saga, which limits how many saga instances are able to concurrently
        /// consume messages.
        /// </summary>
        public int? ConcurrentMessageLimit
        {
            get => _concurrentMessageLimit;
            protected set => _concurrentMessageLimit = value;
        }

        public void Configure(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<FutureState> sagaConfigurator)
        {
            if (_concurrentMessageLimit.HasValue)
                sagaConfigurator.UseConcurrentMessageLimit(_concurrentMessageLimit.Value);

            ConfigureSaga(endpointConfigurator, sagaConfigurator);
        }

        public Type FutureType => typeof(TFuture);

        public string GetEndpointName(IEndpointNameFormatter formatter)
        {
            return string.IsNullOrWhiteSpace(_endpointName)
                ? _endpointName = EndpointDefinition?.GetEndpointName(formatter) ?? formatter.Message<TFuture>()
                : _endpointName;
        }

        /// <summary>
        /// Called when configuring the saga on the endpoint. Configuration only applies to this saga, and does not apply to
        /// the endpoint.
        /// </summary>
        /// <param name="endpointConfigurator">The receive endpoint configurator for the consumer</param>
        /// <param name="sagaConfigurator">The saga configurator</param>
        protected virtual void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<FutureState> sagaConfigurator)
        {
        }

        /// <summary>
        /// Configure the saga endpoint
        /// </summary>
        /// <param name="configure"></param>
        protected void Endpoint(Action<IFutureEndpointRegistrationConfigurator> configure)
        {
            var configurator = new FutureEndpointRegistrationConfigurator<TFuture>();

            configure?.Invoke(configurator);

            EndpointDefinition = new FutureEndpointDefinition<TFuture>(configurator.Settings);
        }
    }
}

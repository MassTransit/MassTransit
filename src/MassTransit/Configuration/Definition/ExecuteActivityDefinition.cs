namespace MassTransit.Definition
{
    using System;
    using ConsumeConfigurators;
    using Courier;
    using Registration;


    public class ExecuteActivityDefinition<TActivity, TArguments> :
        IExecuteActivityDefinition<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        int? _concurrentMessageLimit;
        string _executeEndpointName;

        protected ExecuteActivityDefinition()
        {
        }

        public IEndpointDefinition<IExecuteActivity<TArguments>> ExecuteEndpointDefinition { private get; set; }

        /// <summary>
        /// Specify the endpoint name (which may be a queue, or a subscription, depending upon the transport) on which the saga
        /// should be configured. Setting to null will use the supplied <see cref="IEndpointNameFormatter"/> to generate the
        /// endpoint name.
        /// </summary>
        protected string ExecuteEndpointName
        {
            set => _executeEndpointName = value;
        }

        /// <summary>
        /// Specify a concurrency limit, which is applied to the entire consumer, saga, or activity, regardless of message type.
        /// </summary>
        public int? ConcurrentMessageLimit
        {
            get => _concurrentMessageLimit;
            protected set => _concurrentMessageLimit = value;
        }

        void IExecuteActivityDefinition<TActivity, TArguments>.Configure(IReceiveEndpointConfigurator endpointConfigurator,
            IExecuteActivityConfigurator<TActivity, TArguments> executeActivityConfigurator)
        {
            ConfigureConcurrencyLimit(executeActivityConfigurator.RoutingSlip);

            ConfigureExecuteActivity(endpointConfigurator, executeActivityConfigurator);
        }

        string IExecuteActivityDefinition.GetExecuteEndpointName(IEndpointNameFormatter formatter)
        {
            return string.IsNullOrWhiteSpace(_executeEndpointName)
                ? _executeEndpointName = ExecuteEndpointDefinition?.GetEndpointName(formatter) ?? formatter.ExecuteActivity<TActivity, TArguments>()
                : _executeEndpointName;
        }

        Type IExecuteActivityDefinition.ActivityType => typeof(TActivity);
        Type IExecuteActivityDefinition.ArgumentType => typeof(TArguments);

        /// <summary>
        /// Configure the execute endpoint
        /// </summary>
        /// <param name="configure"></param>
        protected void ExecuteEndpoint(Action<IExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments>> configure)
        {
            var configurator = new ExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments>();

            configure?.Invoke(configurator);

            ExecuteEndpointDefinition = new ExecuteActivityEndpointDefinition<TActivity, TArguments>(configurator.Settings);
        }

        protected void ConfigureConcurrencyLimit(Action<Action<IRoutingSlipConfigurator>> callback)
        {
            if (_concurrentMessageLimit.HasValue)
                callback(x => x.UseConcurrentMessageLimit(_concurrentMessageLimit.Value));
        }

        /// <summary>
        /// Called when the compensate activity is being configured on the endpoint.
        /// </summary>
        /// <param name="endpointConfigurator">The receive endpoint configurator for the consumer</param>
        /// <param name="executeActivityConfigurator"></param>
        protected virtual void ConfigureExecuteActivity(IReceiveEndpointConfigurator endpointConfigurator,
            IExecuteActivityConfigurator<TActivity, TArguments> executeActivityConfigurator)
        {
        }
    }
}

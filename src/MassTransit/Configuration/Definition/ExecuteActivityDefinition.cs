namespace MassTransit.Definition
{
    using System;
    using ConsumeConfigurators;
    using Courier;


    public class ExecuteActivityDefinition<TActivity, TArguments> :
        IExecuteActivityDefinition<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        int? _concurrentMessageLimit;
        string _executeEndpointName;

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
            return !string.IsNullOrWhiteSpace(_executeEndpointName)
                ? _executeEndpointName
                : _executeEndpointName = formatter.ExecuteActivity<TActivity, TArguments>();
        }

        Type IExecuteActivityDefinition.ActivityType => typeof(TActivity);
        Type IExecuteActivityDefinition.ArgumentType => typeof(TArguments);

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

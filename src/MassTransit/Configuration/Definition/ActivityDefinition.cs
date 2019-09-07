namespace MassTransit.Definition
{
    using System;
    using Courier;


    public class ActivityDefinition<TActivity, TArguments, TLog> :
        ExecuteActivityDefinition<TActivity, TArguments>,
        IActivityDefinition<TActivity, TArguments, TLog>
        where TActivity : class, IActivity<TArguments, TLog>
        where TLog : class
        where TArguments : class
    {
        string _compensateEndpointName;

        /// <summary>
        /// Specify the endpoint name (which may be a queue, or a subscription, depending upon the transport) on which the saga
        /// should be configured. Setting to null will use the supplied <see cref="IEndpointNameFormatter"/> to generate the
        /// endpoint name.
        /// </summary>
        protected string CompensateEndpointName
        {
            set => _compensateEndpointName = value;
        }

        void IActivityDefinition<TActivity, TArguments, TLog>.Configure(IReceiveEndpointConfigurator endpointConfigurator,
            ICompensateActivityConfigurator<TActivity, TLog> compensateActivityConfigurator)
        {
            ConfigureConcurrencyLimit(compensateActivityConfigurator.RoutingSlip);

            ConfigureCompensateActivity(endpointConfigurator, compensateActivityConfigurator);
        }

        string IActivityDefinition.GetCompensateEndpointName(IEndpointNameFormatter formatter)
        {
            return !string.IsNullOrWhiteSpace(_compensateEndpointName)
                ? _compensateEndpointName
                : _compensateEndpointName = formatter.CompensateActivity<TActivity, TLog>();
        }

        Type IActivityDefinition.LogType => typeof(TLog);

        /// <summary>
        /// Called when the compensate activity is being configured on the endpoint.
        /// </summary>
        /// <param name="endpointConfigurator">The receive endpoint configurator for the consumer</param>
        /// <param name="compensateActivityConfigurator"></param>
        protected virtual void ConfigureCompensateActivity(IReceiveEndpointConfigurator endpointConfigurator,
            ICompensateActivityConfigurator<TActivity, TLog> compensateActivityConfigurator)
        {
        }
    }
}

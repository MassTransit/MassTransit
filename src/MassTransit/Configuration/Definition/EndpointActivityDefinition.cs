namespace MassTransit.Definition
{
    using System;
    using Courier;


    public class EndpointActivityDefinition<TActivity, TArguments, TLog> :
        EndpointExecuteActivityDefinition<TActivity, TArguments>,
        IActivityDefinition<TActivity, TArguments, TLog>
        where TActivity : class, Activity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly IEndpointDefinition<CompensateActivity<TLog>> _compensateEndpointDefinition;

        public EndpointActivityDefinition(IEndpointDefinition<ExecuteActivity<TArguments>> endpointDefinition,
            IEndpointDefinition<CompensateActivity<TLog>> compensateEndpointDefinition)
            : base(endpointDefinition)
        {
            _compensateEndpointDefinition = compensateEndpointDefinition;
        }

        void IActivityDefinition<TActivity, TArguments, TLog>.Configure(IReceiveEndpointConfigurator endpointConfigurator,
            ICompensateActivityConfigurator<TActivity, TLog> compensateActivityConfigurator)
        {
        }

        Type IActivityDefinition.LogType => typeof(TLog);

        string IActivityDefinition.GetCompensateEndpointName(IEndpointNameFormatter formatter)
        {
            return _compensateEndpointDefinition.GetEndpointName(formatter);
        }
    }
}

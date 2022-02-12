namespace MassTransit
{
    using System;
    using Courier;


    public interface IActivityDefinition :
        IExecuteActivityDefinition
    {
        /// <summary>
        /// The log type
        /// </summary>
        Type LogType { get; }

        IEndpointDefinition? CompensateEndpointDefinition { get; }

        /// <summary>
        /// Return the endpoint name for the compensate activity
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        string GetCompensateEndpointName(IEndpointNameFormatter formatter);
    }


    public interface IActivityDefinition<TActivity, TArguments, TLog> :
        IActivityDefinition,
        IExecuteActivityDefinition<TActivity, TArguments>
        where TActivity : class, IActivity<TArguments, TLog>
        where TLog : class
        where TArguments : class
    {
        /// <summary>
        /// Sets the endpoint definition, if available
        /// </summary>
        new IEndpointDefinition<ICompensateActivity<TLog>> CompensateEndpointDefinition { set; }

        /// <summary>
        /// Configure the compensate activity
        /// </summary>
        /// <param name="endpointConfigurator"></param>
        /// <param name="compensateActivityConfigurator"></param>
        void Configure(IReceiveEndpointConfigurator endpointConfigurator, ICompensateActivityConfigurator<TActivity, TLog> compensateActivityConfigurator);
    }
}

namespace MassTransit
{
    using System;
    using Courier;


    public interface IExecuteActivityDefinition :
        IDefinition
    {
        /// <summary>
        /// The Activity type
        /// </summary>
        Type ActivityType { get; }

        /// <summary>
        /// The argument type
        /// </summary>
        Type ArgumentType { get; }

        IEndpointDefinition? ExecuteEndpointDefinition { get; }

        /// <summary>
        /// Return the endpoint name for the execute activity
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        string GetExecuteEndpointName(IEndpointNameFormatter formatter);
    }


    public interface IExecuteActivityDefinition<TActivity, TArguments> :
        IExecuteActivityDefinition
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        /// <summary>
        /// Sets the endpoint definition, if available
        /// </summary>
        new IEndpointDefinition<IExecuteActivity<TArguments>>? ExecuteEndpointDefinition { set; }

        /// <summary>
        /// Configure the execute activity
        /// </summary>
        /// <param name="endpointConfigurator"></param>
        /// <param name="executeActivityConfigurator"></param>
        void Configure(IReceiveEndpointConfigurator endpointConfigurator, IExecuteActivityConfigurator<TActivity, TArguments> executeActivityConfigurator);
    }
}

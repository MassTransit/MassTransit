namespace MassTransit
{
    using System;
    using Courier;
    using Registration;


    public interface IActivityRegistrationConfigurator<TActivity, TArguments, TLog>
        where TActivity : class, IActivity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        /// <summary>
        /// Configure both the execute and compensate endpoints in a single call. Separate calls have been added, which
        /// may ultimately cause this method to be deprecated.
        /// </summary>
        /// <param name="configureExecute"></param>
        /// <param name="configureCompensate"></param>
        void Endpoints(Action<IExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments>> configureExecute,
            Action<ICompensateActivityEndpointRegistrationConfigurator<TActivity, TLog>> configureCompensate);

        /// <summary>
        /// Configure the activity's execute endpoint
        /// </summary>
        /// <param name="configureExecute"></param>
        void ExecuteEndpoint(Action<IExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments>> configureExecute);

        /// <summary>
        /// Configure the activity's compensate endpoint
        /// </summary>
        /// <param name="configureCompensate"></param>
        void CompensateEndpoint(Action<ICompensateActivityEndpointRegistrationConfigurator<TActivity, TLog>> configureCompensate);
    }
}

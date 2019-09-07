namespace MassTransit
{
    using System;
    using ConsumeConfigurators;
    using Courier;
    using GreenPipes;


    /// <summary>
    /// Configure the execution of the activity and arguments with some tasty middleware.
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TArguments"></typeparam>
    public interface IExecuteActivityConfigurator<TActivity, TArguments> :
        IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>>,
        IConsumeConfigurator
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        /// <summary>
        /// Configure the arguments separate from the activity
        /// </summary>
        void Arguments(Action<IExecuteActivityArgumentsConfigurator<TArguments>> configure);

        /// <summary>
        /// Configure the routing slip pipe
        /// </summary>
        void RoutingSlip(Action<IRoutingSlipConfigurator> configure);
    }
}

namespace MassTransit.ConsumeConfigurators
{
    using System;
    using Courier;


    public interface IActivityConfigurationObserver
    {
        /// <summary>
        /// Called when a routing slip activity that supports compensation host is configured
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="compensateAddress">The address of the compensation endpoint</param>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class;

        /// <summary>
        /// Called when a routing slip execute activity host is configured
        /// </summary>
        /// <param name="configurator"></param>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class;

        /// <summary>
        /// Called when a routing slip compensate activity host is configured
        /// </summary>
        /// <param name="configurator"></param>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TLog"></typeparam>
        void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class;
    }
}

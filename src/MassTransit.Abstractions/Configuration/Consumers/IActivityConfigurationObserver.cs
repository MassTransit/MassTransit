namespace MassTransit
{
    using System;
    using System.ComponentModel;
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class;

        /// <summary>
        /// Called when a routing slip execute activity host is configured
        /// </summary>
        /// <param name="configurator"></param>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class;

        /// <summary>
        /// Called when a routing slip compensate activity host is configured
        /// </summary>
        /// <param name="configurator"></param>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TLog"></typeparam>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class;
    }
}

namespace MassTransit.Configuration
{
    using System;
    using Courier;
    using Util;


    public class ActivityConfigurationObservable :
        Connectable<IActivityConfigurationObserver>,
        IActivityConfigurationObserver
    {
        public void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            ForEach(observer => observer.ActivityConfigured(configurator, compensateAddress));
        }

        public void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            ForEach(observer => observer.ExecuteActivityConfigured(configurator));
        }

        public void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            ForEach(observer => observer.CompensateActivityConfigured(configurator));
        }
    }
}

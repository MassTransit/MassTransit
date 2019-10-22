namespace MassTransit.ConsumeConfigurators
{
    using System;
    using Courier;
    using GreenPipes.Util;


    public class ActivityConfigurationObservable :
        Connectable<IActivityConfigurationObserver>,
        IActivityConfigurationObserver
    {
        public void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            All(observer =>
            {
                observer.ActivityConfigured(configurator, compensateAddress);

                return true;
            });
        }

        public void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            All(observer =>
            {
                observer.ExecuteActivityConfigured(configurator);

                return true;
            });
        }

        public void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            All(observer =>
            {
                observer.CompensateActivityConfigured(configurator);

                return true;
            });
        }
    }
}

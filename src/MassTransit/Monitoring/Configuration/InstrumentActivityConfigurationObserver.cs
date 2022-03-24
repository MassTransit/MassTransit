namespace MassTransit.Monitoring.Configuration
{
    using System;


    public class InstrumentActivityConfigurationObserver :
        IActivityConfigurationObserver
    {
        readonly IActivityObserver _observer;

        public InstrumentActivityConfigurationObserver()
        {
            _observer = new InstrumentActivityObserver();
        }

        public void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var specification = new InstrumentExecuteActivitySpecification<TActivity, TArguments>();

            configurator.AddPipeSpecification(specification);

            configurator.ConnectActivityObserver(_observer);
        }

        public void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var specification = new InstrumentExecuteActivitySpecification<TActivity, TArguments>();

            configurator.AddPipeSpecification(specification);

            configurator.ConnectActivityObserver(_observer);
        }

        public void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var specification = new InstrumentCompensateActivitySpecification<TActivity, TLog>();

            configurator.AddPipeSpecification(specification);

            configurator.ConnectActivityObserver(_observer);
        }
    }
}

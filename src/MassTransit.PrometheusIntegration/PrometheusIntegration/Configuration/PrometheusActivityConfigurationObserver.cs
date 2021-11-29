namespace MassTransit.PrometheusIntegration.Configuration
{
    using System;
    using Courier;


    public class PrometheusActivityConfigurationObserver :
        IActivityConfigurationObserver
    {
        readonly IActivityObserver _observer;

        public PrometheusActivityConfigurationObserver()
        {
            _observer = new PrometheusActivityObserver();
        }

        public void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var specification = new PrometheusExecuteActivitySpecification<TActivity, TArguments>();

            configurator.AddPipeSpecification(specification);

            configurator.ConnectActivityObserver(_observer);
        }

        public void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var specification = new PrometheusExecuteActivitySpecification<TActivity, TArguments>();

            configurator.AddPipeSpecification(specification);

            configurator.ConnectActivityObserver(_observer);
        }

        public void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var specification = new PrometheusCompensateActivitySpecification<TActivity, TLog>();

            configurator.AddPipeSpecification(specification);

            configurator.ConnectActivityObserver(_observer);
        }
    }
}

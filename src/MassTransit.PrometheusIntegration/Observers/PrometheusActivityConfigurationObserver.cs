namespace MassTransit.PrometheusIntegration.Observers
{
    using System;
    using Configuration;
    using ConsumeConfigurators;
    using Courier;


    public class PrometheusActivityConfigurationObserver :
        IActivityConfigurationObserver
    {
        public void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var specification = new PrometheusExecuteActivitySpecification<TActivity, TArguments>();

            configurator.AddPipeSpecification(specification);
        }

        public void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var specification = new PrometheusExecuteActivitySpecification<TActivity, TArguments>();

            configurator.AddPipeSpecification(specification);
        }

        public void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var specification = new PrometheusCompensateActivitySpecification<TActivity, TLog>();

            configurator.AddPipeSpecification(specification);
        }
    }
}

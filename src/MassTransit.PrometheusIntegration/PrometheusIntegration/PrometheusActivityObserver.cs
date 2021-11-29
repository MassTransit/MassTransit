namespace MassTransit.PrometheusIntegration
{
    using System;
    using System.Threading.Tasks;
    using Courier;


    public class PrometheusActivityObserver :
        IActivityObserver
    {
        public Task PreExecute<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return Task.CompletedTask;
        }

        public Task PostExecute<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            PrometheusMetrics.MeasureExecute(context);

            return Task.CompletedTask;
        }

        public Task ExecuteFault<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context, Exception exception)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            PrometheusMetrics.MeasureExecute(context, exception);

            return Task.CompletedTask;
        }

        public Task PreCompensate<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            return Task.CompletedTask;
        }

        public Task PostCompensate<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            PrometheusMetrics.MeasureCompensate(context);

            return Task.CompletedTask;
        }

        public Task CompensateFail<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context, Exception exception)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            PrometheusMetrics.MeasureCompensate(context, exception);

            return Task.CompletedTask;
        }
    }
}

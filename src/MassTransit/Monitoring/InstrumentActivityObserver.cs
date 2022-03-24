namespace MassTransit.Monitoring
{
    using System;
    using System.Threading.Tasks;


    public class InstrumentActivityObserver :
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
            Instrumentation.MeasureExecute(context);

            return Task.CompletedTask;
        }

        public Task ExecuteFault<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context, Exception exception)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            Instrumentation.MeasureExecute(context, exception);

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
            Instrumentation.MeasureCompensate(context);

            return Task.CompletedTask;
        }

        public Task CompensateFail<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context, Exception exception)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            Instrumentation.MeasureCompensate(context, exception);

            return Task.CompletedTask;
        }
    }
}

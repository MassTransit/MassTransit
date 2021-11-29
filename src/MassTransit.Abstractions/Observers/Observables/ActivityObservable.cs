namespace MassTransit.Observables
{
    using System;
    using System.Threading.Tasks;
    using Courier;
    using Util;


    public class ActivityObservable :
        Connectable<IActivityObserver>,
        IActivityObserver
    {
        public Task PreExecute<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return ForEachAsync(x => x.PreExecute(context));
        }

        public Task PostExecute<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return ForEachAsync(x => x.PostExecute(context));
        }

        public Task ExecuteFault<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context, Exception exception)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return ForEachAsync(x => x.ExecuteFault(context, exception));
        }

        public Task PreCompensate<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            return ForEachAsync(x => x.PreCompensate(context));
        }

        public Task PostCompensate<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            return ForEachAsync(x => x.PostCompensate(context));
        }

        public Task CompensateFail<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context, Exception exception)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            return ForEachAsync(x => x.CompensateFail(context, exception));
        }
    }
}

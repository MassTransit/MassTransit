namespace MassTransit.Courier
{
    using System;


    [Obsolete("While this still works, consider replacing with the IActivityFactory<TActivity, TArguments, TLog> interface instead")]
    public interface ActivityFactory<out TActivity, TArguments, TLog> :
        IActivityFactory<TActivity, TArguments, TLog>
        where TActivity : class, IExecuteActivity<TArguments>, ICompensateActivity<TLog>
        where TArguments : class
        where TLog : class
    {
    }
}

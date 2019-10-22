namespace MassTransit.Courier
{
    using System;


    [Obsolete("While this still works, consider replacing with the IActivity<TActivity, TArguments, TLog> interface instead")]
    public interface Activity<in TArguments, in TLog> :
        IActivity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
    }
}

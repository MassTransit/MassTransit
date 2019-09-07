namespace MassTransit.Courier
{
    using System;


    [Obsolete("While this still works, consider replacing with the ICompensateActivity<TLog> interface instead")]
    public interface CompensateActivity<in TLog> :
        ICompensateActivity<TLog>
        where TLog : class
    {
    }
}

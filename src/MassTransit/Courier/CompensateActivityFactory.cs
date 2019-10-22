namespace MassTransit.Courier
{
    using System;


    [Obsolete("While this still works, consider replacing with the ICompensateActivityFactory<TActivity, TLog> interface instead")]
    public interface CompensateActivityFactory<out TActivity, TLog> :
        ICompensateActivityFactory<TActivity, TLog>
        where TLog : class
        where TActivity : class, ICompensateActivity<TLog>
    {
    }
}

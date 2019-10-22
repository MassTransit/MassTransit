namespace MassTransit.Courier
{
    using System;


    [Obsolete("While this still works, consider replacing with the IExecuteActivityFactory<TActivity, TArguments> interface instead")]
    public interface ExecuteActivityFactory<out TActivity, TArguments> :
        IExecuteActivityFactory<TActivity, TArguments>
        where TArguments : class
        where TActivity : class, IExecuteActivity<TArguments>
    {
    }
}

namespace MassTransit.Courier
{
    using System;


    [Obsolete("While this still works, consider replacing with the IExecuteActivity<TArguments> interface instead")]
    public interface ExecuteActivity<in TArguments> :
        IExecuteActivity<TArguments>
        where TArguments : class
    {
    }
}

namespace MassTransit.Scoping
{
    using System;
    using Courier;


    public interface ICompensateActivityScopeContext<out TActivity, out TLog> :
        IDisposable
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        CompensateActivityContext<TActivity, TLog> Context { get; }
    }
}

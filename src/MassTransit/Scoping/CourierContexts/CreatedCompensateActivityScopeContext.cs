namespace MassTransit.Scoping.CourierContexts
{
    using System;
    using Courier;


    public class CreatedCompensateActivityScopeContext<TScope, TActivity, TLog> :
        ICompensateActivityScopeContext<TActivity, TLog>
        where TScope : IDisposable
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly TScope _scope;
        readonly Action<TActivity> _disposeCallback;

        public CreatedCompensateActivityScopeContext(TScope scope, CompensateActivityContext<TActivity, TLog> context, Action<TActivity> disposeCallback = null)
        {
            _scope = scope;
            _disposeCallback = disposeCallback;
            Context = context;
        }

        public void Dispose()
        {
            _disposeCallback?.Invoke(Context.Activity);
            _scope.Dispose();
        }

        public CompensateActivityContext<TActivity, TLog> Context { get; }
    }
}

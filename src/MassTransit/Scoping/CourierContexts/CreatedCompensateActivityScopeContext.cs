namespace MassTransit.Scoping.CourierContexts
{
    using System;
    using Courier;


    public class CreatedCompensateActivityScopeContext<TScope, TActivity, TLog> :
        ICompensateActivityScopeContext<TActivity, TLog>
        where TScope : IDisposable
        where TActivity : class, CompensateActivity<TLog>
        where TLog : class
    {
        readonly TScope _scope;

        public CreatedCompensateActivityScopeContext(TScope scope, CompensateActivityContext<TActivity, TLog> context)
        {
            _scope = scope;
            Context = context;
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public CompensateActivityContext<TActivity, TLog> Context { get; }
    }
}
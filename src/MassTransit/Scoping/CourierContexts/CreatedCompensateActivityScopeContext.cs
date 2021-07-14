namespace MassTransit.Scoping.CourierContexts
{
    using System;
    using System.Threading.Tasks;
    using Courier;


    public class CreatedCompensateActivityScopeContext<TScope, TActivity, TLog> :
        ICompensateActivityScopeContext<TActivity, TLog>
        where TScope : IDisposable
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly Action<TActivity> _disposeCallback;
        readonly TScope _scope;

        public CreatedCompensateActivityScopeContext(TScope scope, CompensateActivityContext<TActivity, TLog> context, Action<TActivity> disposeCallback = null)
        {
            _scope = scope;
            _disposeCallback = disposeCallback;
            Context = context;
        }

        public CompensateActivityContext<TActivity, TLog> Context { get; }

        public async ValueTask DisposeAsync()
        {
            _disposeCallback?.Invoke(Context.Activity);

            if (_scope is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            else
                _scope?.Dispose();
        }
    }
}

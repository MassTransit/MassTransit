namespace MassTransit.Scoping.CourierContexts
{
    using Courier;


    public class ExistingCompensateActivityScopeContext<TActivity, TLog> :
        ICompensateActivityScopeContext<TActivity, TLog>
        where TActivity : class, CompensateActivity<TLog>
        where TLog : class
    {
        public ExistingCompensateActivityScopeContext(CompensateActivityContext<TActivity, TLog> context)
        {
            Context = context;
        }

        public void Dispose()
        {
        }

        public CompensateActivityContext<TActivity, TLog> Context { get; }
    }
}
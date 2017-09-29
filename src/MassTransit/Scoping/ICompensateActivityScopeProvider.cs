namespace MassTransit.Scoping
{
    using Courier;
    using GreenPipes;


    public interface ICompensateActivityScopeProvider<out TActivity, TLog> :
        IProbeSite
        where TActivity : class, CompensateActivity<TLog>
        where TLog : class
    {
        ICompensateActivityScopeContext<TActivity, TLog> GetScope(CompensateContext<TLog> context);
    }
}
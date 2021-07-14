namespace MassTransit.Scoping
{
    using System.Threading.Tasks;
    using Courier;
    using GreenPipes;


    public interface ICompensateActivityScopeProvider<TActivity, TLog> :
        IProbeSite
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        ValueTask<ICompensateActivityScopeContext<TActivity, TLog>> GetScope(CompensateContext<TLog> context);
    }
}

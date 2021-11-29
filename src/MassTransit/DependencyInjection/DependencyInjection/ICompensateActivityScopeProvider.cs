namespace MassTransit.DependencyInjection
{
    using System.Threading.Tasks;
    using Courier;


    public interface ICompensateActivityScopeProvider<TActivity, TLog> :
        IProbeSite
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        ValueTask<ICompensateScopeContext<TLog>> GetScope(CompensateContext<TLog> context);

        ValueTask<ICompensateActivityScopeContext<TActivity, TLog>> GetActivityScope(CompensateContext<TLog> context);
    }
}

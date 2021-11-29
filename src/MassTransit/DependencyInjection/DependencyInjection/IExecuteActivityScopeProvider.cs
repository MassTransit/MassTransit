namespace MassTransit.DependencyInjection
{
    using System.Threading.Tasks;
    using Courier;


    public interface IExecuteActivityScopeProvider<TActivity, TArguments> :
        IProbeSite
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        ValueTask<IExecuteScopeContext<TArguments>> GetScope(ExecuteContext<TArguments> context);

        ValueTask<IExecuteActivityScopeContext<TActivity, TArguments>> GetActivityScope(ExecuteContext<TArguments> context);
    }
}

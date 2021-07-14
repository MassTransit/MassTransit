namespace MassTransit.Scoping
{
    using System.Threading.Tasks;
    using Courier;
    using GreenPipes;


    public interface IExecuteActivityScopeProvider<TActivity, TArguments> :
        IProbeSite
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        ValueTask<IExecuteActivityScopeContext<TActivity, TArguments>> GetScope(ExecuteContext<TArguments> context);
    }
}

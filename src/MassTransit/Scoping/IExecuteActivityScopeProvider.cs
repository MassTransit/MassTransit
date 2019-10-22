namespace MassTransit.Scoping
{
    using Courier;
    using GreenPipes;


    public interface IExecuteActivityScopeProvider<out TActivity, TArguments> :
        IProbeSite
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        IExecuteActivityScopeContext<TActivity, TArguments> GetScope(ExecuteContext<TArguments> context);
    }
}

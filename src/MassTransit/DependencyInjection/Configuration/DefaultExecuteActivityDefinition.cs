namespace MassTransit.Configuration
{
    public class DefaultExecuteActivityDefinition<TActivity, TArguments> :
        ExecuteActivityDefinition<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
    }
}

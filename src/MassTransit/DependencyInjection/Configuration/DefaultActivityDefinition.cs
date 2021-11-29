namespace MassTransit.Configuration
{
    using Courier;


    public class DefaultActivityDefinition<TActivity, TArguments, TLog> :
        ActivityDefinition<TActivity, TArguments, TLog>
        where TActivity : class, IActivity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
    }
}

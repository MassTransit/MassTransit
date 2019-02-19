namespace MassTransit.Definition
{
    using Courier;


    public class DefaultActivityDefinition<TActivity, TArguments, TLog> :
        ActivityDefinition<TActivity, TArguments, TLog>
        where TActivity : class, Activity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
    }
}

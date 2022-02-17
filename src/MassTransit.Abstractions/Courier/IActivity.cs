namespace MassTransit
{
    /// <summary>
    /// An Activity implements the execute and compensate methods for an activity
    /// </summary>
    /// <typeparam name="TArguments">The activity argument type</typeparam>
    /// <typeparam name="TLog">The activity log argument type</typeparam>
    public interface IActivity<in TArguments, in TLog> :
        IExecuteActivity<TArguments>,
        ICompensateActivity<TLog>,
        IActivity
        where TLog : class
        where TArguments : class
    {
    }


    /// <summary>
    /// Marker interface used to assist identification in IoC containers.
    /// Not to be used directly as it does not contain the message type of the
    /// consumer
    /// </summary>
    /// <remarks>
    /// Not to be used directly by application code, for internal reflection only
    /// </remarks>
    public interface IActivity
    {
    }
}

namespace MassTransit
{
    using Courier;


    public interface ExecuteActivityContext<out TArguments> :
        ExecuteContext<TArguments>
        where TArguments : class
    {
    }


    /// <summary>
    /// An activity and execution context combined into a single container from the factory
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TArguments"></typeparam>
    public interface ExecuteActivityContext<out TActivity, out TArguments> :
        ExecuteActivityContext<TArguments>
        where TArguments : class
        where TActivity : class, IExecuteActivity<TArguments>
    {
        /// <summary>
        /// The activity that was created/used for this execution
        /// </summary>
        TActivity Activity { get; }
    }
}

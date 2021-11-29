namespace MassTransit
{
    /// <summary>
    /// Configure the execution of the activity and arguments with some tasty middleware.
    /// </summary>
    /// <typeparam name="TArguments"></typeparam>
    public interface IExecuteArgumentsConfigurator<TArguments> :
        IPipeConfigurator<ExecuteContext<TArguments>>,
        IConsumeConfigurator
        where TArguments : class
    {
    }
}

namespace MassTransit
{
    using ConsumeConfigurators;
    using Courier;
    using GreenPipes;


    /// <summary>
    /// Configure the execution of the activity and arguments with some tasty middleware.
    /// </summary>
    /// <typeparam name="TLog"></typeparam>
    public interface ICompensateLogConfigurator<TLog> :
        IPipeConfigurator<CompensateContext<TLog>>,
        IConsumeConfigurator
        where TLog : class
    {
    }
}

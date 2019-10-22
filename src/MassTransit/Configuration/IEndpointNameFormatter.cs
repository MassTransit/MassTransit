namespace MassTransit
{
    using Courier;
    using Saga;


    public interface IEndpointNameFormatter
    {
        /// <summary>
        /// Generate a temporary endpoint name, containing the specified tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        string TemporaryEndpoint(string tag);

        string Consumer<T>()
            where T : class, IConsumer;

        string Saga<T>()
            where T : class, ISaga;

        string ExecuteActivity<T, TArguments>()
            where T : class, IExecuteActivity<TArguments>
            where TArguments : class;

        string CompensateActivity<T, TLog>()
            where T : class, ICompensateActivity<TLog>
            where TLog : class;
    }
}

namespace MassTransit
{
    using Courier;


    public interface IEndpointNameFormatter
    {
        /// <summary>
        /// The separator string used between words
        /// </summary>
        string Separator { get; }

        /// <summary>
        /// Generate a temporary endpoint name, containing the specified tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        string TemporaryEndpoint(string tag);

        string Consumer<T>()
            where T : class, IConsumer;

        string Message<T>()
            where T : class;

        string Saga<T>()
            where T : class, ISaga;

        string ExecuteActivity<T, TArguments>()
            where T : class, IExecuteActivity<TArguments>
            where TArguments : class;

        string CompensateActivity<T, TLog>()
            where T : class, ICompensateActivity<TLog>
            where TLog : class;

        /// <summary>
        /// Clean up a name so that it matches the formatting.
        /// For instance, SubmitOrderControl -> submit-order-control (kebab case)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string SanitizeName(string name);
    }
}

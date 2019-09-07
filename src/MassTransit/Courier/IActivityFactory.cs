namespace MassTransit.Courier
{
    using System.Threading.Tasks;
    using GreenPipes;


    public interface IActivityFactory<out TActivity, TArguments, TLog> :
        IExecuteActivityFactory<TActivity, TArguments>,
        ICompensateActivityFactory<TActivity, TLog>
        where TActivity : class, IExecuteActivity<TArguments>, ICompensateActivity<TLog>
        where TArguments : class
        where TLog : class
    {
    }


    /// <summary>
    /// Should be implemented by containers that support generic object resolution in order to
    /// provide a common lifetime management policy for all activities
    /// </summary>
    public interface IActivityFactory :
        IProbeSite
    {
        /// <summary>
        /// Create and execute the activity
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        Task<ResultContext<ExecutionResult>> Execute<TActivity, TArguments>(ExecuteContext<TArguments> context,
            IRequestPipe<ExecuteActivityContext<TActivity, TArguments>, ExecutionResult> next)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class;

        /// <summary>
        /// Create and compensate the activity
        /// </summary>
        /// <param name="compensateContext"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        Task<ResultContext<CompensationResult>> Compensate<TActivity, TLog>(CompensateContext<TLog> compensateContext,
            IRequestPipe<CompensateActivityContext<TActivity, TLog>, CompensationResult> next)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class;
    }
}

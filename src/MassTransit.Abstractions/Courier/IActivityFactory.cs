namespace MassTransit
{
    using System.Threading.Tasks;


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
        Task Execute<TActivity, TArguments>(ExecuteContext<TArguments> context, IPipe<ExecuteActivityContext<TActivity, TArguments>> next)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class;

        /// <summary>
        /// Create and compensate the activity
        /// </summary>
        /// <param name="compensateContext"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        Task Compensate<TActivity, TLog>(CompensateContext<TLog> compensateContext, IPipe<CompensateActivityContext<TActivity, TLog>> next)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class;
    }
}

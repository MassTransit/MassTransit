namespace MassTransit.Courier
{
    using Factories;


    public static class ActivityFactoryExtensions
    {
        /// <summary>
        /// Created an activity factory for the specified activity type
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        /// <typeparam name="TLog"></typeparam>
        /// <param name="activityFactory"></param>
        /// <returns></returns>
        public static IActivityFactory<TActivity, TArguments, TLog> CreateActivityFactory<TActivity, TArguments, TLog>(
            this IActivityFactory activityFactory)
            where TActivity : class, IExecuteActivity<TArguments>, ICompensateActivity<TLog>
            where TArguments : class
            where TLog : class
        {
            return new GenericActivityFactory<TActivity, TArguments, TLog>(activityFactory);
        }
    }
}

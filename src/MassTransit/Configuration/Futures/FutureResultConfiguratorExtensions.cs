namespace MassTransit
{
    public static class FutureResultConfiguratorExtensions
    {
        public static void SetCompleted<TResult, TInput>(this IFutureResultConfigurator<TResult, TInput> configurator)
            where TResult : class
            where TInput : class, TResult
        {
            configurator.SetCompletedUsingFactory(context => context.Message);
        }
    }
}

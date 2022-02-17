namespace MassTransit.Courier
{
    public static class DefaultConstructorExecuteActivityFactory<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>, new()
        where TArguments : class
    {
        public static IExecuteActivityFactory<TActivity, TArguments> ExecuteFactory => ActivityFactoryCache.Factory;


        static class ActivityFactoryCache
        {
            internal static readonly IExecuteActivityFactory<TActivity, TArguments> Factory =
                new FactoryMethodExecuteActivityFactory<TActivity, TArguments>(_ => new TActivity());
        }
    }
}

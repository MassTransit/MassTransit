namespace MassTransit.Courier
{
    public static class DefaultConstructorCompensateActivityFactory<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>, new()
        where TLog : class
    {
        public static ICompensateActivityFactory<TActivity, TLog> CompensateFactory => ActivityFactoryCache.Factory;


        static class ActivityFactoryCache
        {
            internal static readonly ICompensateActivityFactory<TActivity, TLog> Factory =
                new FactoryMethodCompensateActivityFactory<TActivity, TLog>(_ => new TActivity());
        }
    }
}

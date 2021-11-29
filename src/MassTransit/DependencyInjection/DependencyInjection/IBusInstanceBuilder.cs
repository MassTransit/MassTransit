namespace MassTransit.DependencyInjection
{
    public interface IBusInstanceBuilder
    {
        TResult GetBusInstanceType<TBus, TResult>(IBusInstanceBuilderCallback<TBus, TResult> callback)
            where TBus : class, IBus;
    }
}

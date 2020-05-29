namespace MassTransit.Internals.Reflection
{
    using MultiBus;


    public interface IBusInstanceBuilderCallback<TBus, out TResult>
        where TBus : class, IBus
    {
        TResult GetResult<TBusInstance>()
            where TBusInstance : BusInstance<TBus>, TBus;
    }
}

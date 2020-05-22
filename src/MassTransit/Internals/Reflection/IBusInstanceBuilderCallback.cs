namespace MassTransit.Internals.Reflection
{
    using MultiBus;
    using Registration;


    public interface IBusInstanceBuilderCallback<TBus, out TResult>
        where TBus : class, IBus
    {
        TResult GetResult<TBusInstance>()
            where TBusInstance : BusInstance<TBus>, TBus;
    }
}

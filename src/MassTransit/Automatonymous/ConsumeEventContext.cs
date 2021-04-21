namespace Automatonymous
{
    using MassTransit;


    /// <summary>
    /// Combines the consumption of an event in a state machine with the consumer context of the receiving endpoint.
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public interface ConsumeEventContext<out TInstance> :
        EventContext<TInstance>,
        ConsumeContext
    {
    }


    /// <summary>
    /// Combines the consumption of an event in a state machine with the consumer context of the receiving endpoint.
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public interface ConsumeEventContext<out TInstance, out TData> :
        EventContext<TInstance, TData>,
        ConsumeContext
        where TData : class
    {
        ConsumeEventContext<TInstance> InstanceContext { get; }
    }
}

namespace Automatonymous
{
    /// <summary>
    /// Combines the consumption of an event in a state machine with the consumer context of the receiving endpoint.
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TException"></typeparam>
    public interface ConsumeExceptionEventContext<out TInstance, out TException> :
        ConsumeEventContext<TInstance>
    {
        TException Exception { get; }
    }


    /// <summary>
    /// Combines the consumption of an event in a state machine with the consumer context of the receiving endpoint.
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TException"></typeparam>
    public interface ConsumeExceptionEventContext<out TInstance, out TData, out TException> :
        ConsumeEventContext<TInstance, TData>
        where TData : class
    {
        TException Exception { get; }
    }
}

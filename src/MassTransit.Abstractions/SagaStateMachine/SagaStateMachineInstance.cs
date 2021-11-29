namespace MassTransit
{
    /// <summary>
    /// An Automatonymous state machine instance that is usable as a saga by MassTransit must implement this interface.
    /// It indicates to the framework the available features of the state as being a state machine instance.
    /// </summary>
    public interface SagaStateMachineInstance :
        ISaga
    {
    }
}

namespace MassTransit
{
    using System;


    [Serializable]
    public class NotAcceptedStateMachineException :
        SagaException
    {
        public NotAcceptedStateMachineException(Type sagaType, Type messageType, Guid correlationId, string currentState, Exception exception)
            : base($"Not accepted in state {currentState}", sagaType, messageType, correlationId, exception)
        {
        }
    }
}

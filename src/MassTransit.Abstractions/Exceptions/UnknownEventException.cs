namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class UnknownEventException :
        SagaStateMachineException
    {
        public UnknownEventException()
        {
        }

        public UnknownEventException(string machineType, string eventName)
            : base($"The {eventName} event is not defined for the {machineType} state machine")
        {
        }

        protected UnknownEventException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

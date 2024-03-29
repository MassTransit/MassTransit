namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class UnhandledEventException :
        SagaStateMachineException
    {
        public UnhandledEventException()
        {
        }

        public UnhandledEventException(string machineType, string eventName, string stateName)
            : base($"The {eventName} event is not handled during the {stateName} state for the {machineType} state machine")
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected UnhandledEventException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class UnknownStateException :
        SagaStateMachineException
    {
        public UnknownStateException()
        {
        }

        public UnknownStateException(string machineType, string stateName)
            : base($"The {stateName} state is not defined for the {machineType} state machine")
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected UnknownStateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

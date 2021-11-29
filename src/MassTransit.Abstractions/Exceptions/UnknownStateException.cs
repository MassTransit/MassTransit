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

        protected UnknownStateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

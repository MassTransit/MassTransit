namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class SagaStateMachineException :
        MassTransitException
    {
        public SagaStateMachineException()
        {
        }

        public SagaStateMachineException(string message)
            : base(message)
        {
        }

        public SagaStateMachineException(Type machineType, string message)
            : base($"{machineType.Name}: {message}")
        {
        }

        public SagaStateMachineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public SagaStateMachineException(Type machineType, string message, Exception innerException)
            : base($"{machineType.Name}: {message}", innerException)
        {
        }

        protected SagaStateMachineException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

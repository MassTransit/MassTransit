namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class EventExecutionException :
        SagaStateMachineException
    {
        public EventExecutionException(string message)
            : base(message)
        {
        }

        public EventExecutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected EventExecutionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public EventExecutionException()
        {
        }
    }
}

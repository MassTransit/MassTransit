namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class ActivityExecutionFaultedException :
        ActivityExecutionException
    {
        public ActivityExecutionFaultedException()
            : this("The routing slip activity execution faulted with an unspecified exception")
        {
        }

        public ActivityExecutionFaultedException(string message)
            : base(message)
        {
        }

        public ActivityExecutionFaultedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ActivityExecutionFaultedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

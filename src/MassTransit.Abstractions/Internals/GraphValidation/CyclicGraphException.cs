namespace MassTransit.Internals.GraphValidation
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class CyclicGraphException :
        MassTransitException
    {
        public CyclicGraphException()
        {
        }

        public CyclicGraphException(string message)
            : base(message)
        {
        }

        public CyclicGraphException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CyclicGraphException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

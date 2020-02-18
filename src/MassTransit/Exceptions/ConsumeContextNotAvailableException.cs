namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class ConsumeContextNotAvailableException :
        MassTransitException
    {
        public ConsumeContextNotAvailableException()
            : this("A valid ConsumeContext was not available")
        {
        }

        public ConsumeContextNotAvailableException(string message)
            : base(message)
        {
        }

        public ConsumeContextNotAvailableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ConsumeContextNotAvailableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

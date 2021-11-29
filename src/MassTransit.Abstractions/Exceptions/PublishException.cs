namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class PublishException :
        MassTransitException
    {
        public PublishException()
        {
        }

        public PublishException(string message)
            : base(message)
        {
        }

        public PublishException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PublishException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class OutboxException :
        MassTransitException
    {
        public OutboxException()
        {
        }

        public OutboxException(string message)
            : base(message)
        {
        }

        public OutboxException(string message, Exception innerException)
            :
            base(message, innerException)
        {
        }

        protected OutboxException(SerializationInfo info, StreamingContext context)
            :
            base(info, context)
        {
        }
    }
}

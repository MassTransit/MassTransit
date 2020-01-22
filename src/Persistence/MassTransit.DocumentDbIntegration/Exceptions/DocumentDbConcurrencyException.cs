namespace MassTransit.DocumentDbIntegration
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class DocumentDbConcurrencyException :
        MassTransitException
    {
        public DocumentDbConcurrencyException()
        {
        }

        public DocumentDbConcurrencyException(string message)
            : base(message)
        {
        }

        public DocumentDbConcurrencyException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public DocumentDbConcurrencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

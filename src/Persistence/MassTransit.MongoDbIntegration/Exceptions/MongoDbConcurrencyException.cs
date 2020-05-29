namespace MassTransit.MongoDbIntegration
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class MongoDbConcurrencyException :
        MassTransitException
    {
        public MongoDbConcurrencyException()
        {
        }

        public MongoDbConcurrencyException(string message)
            : base(message)
        {
        }

        public MongoDbConcurrencyException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public MongoDbConcurrencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

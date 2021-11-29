namespace MassTransit
{
    using System;


    [Serializable]
    public class MongoDbConcurrencyException :
        ConcurrencyException
    {
        public MongoDbConcurrencyException(string message, Type sagaType, Guid correlationId)
            : base(message, sagaType, correlationId)
        {
        }

        public MongoDbConcurrencyException(string message, Type sagaType, Guid correlationId, Exception innerException)
            : base(message, sagaType, correlationId, innerException)
        {
        }

        public MongoDbConcurrencyException()
        {
        }
    }
}

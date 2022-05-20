namespace MassTransit
{
    using System;


    [Serializable]
    public class DynamoDbSagaConcurrencyException :
        ConcurrencyException
    {
        public DynamoDbSagaConcurrencyException(string message, Type sagaType, Guid correlationId)
            : base(message, sagaType, correlationId)
        {
        }

        public DynamoDbSagaConcurrencyException(string message, Type sagaType, Guid correlationId, Exception innerException)
            : base(message, sagaType, correlationId, innerException)
        {
        }

        public DynamoDbSagaConcurrencyException()
        {
        }
    }
}

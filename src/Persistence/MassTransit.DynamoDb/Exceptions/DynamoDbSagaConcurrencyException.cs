namespace MassTransit.DynamoDb.Exceptions
{
    using System;


    [Serializable]
    public class DynamoDbSagaConcurrencyException :
        SagaException
    {
        public DynamoDbSagaConcurrencyException()
        {
        }

        public DynamoDbSagaConcurrencyException(string message, Type sagaType, Guid correlationId)
            : base(message, sagaType, correlationId)
        {
        }
    }
}

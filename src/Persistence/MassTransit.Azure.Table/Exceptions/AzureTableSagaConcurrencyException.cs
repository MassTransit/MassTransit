namespace MassTransit.Azure.Table
{
    using System;


    [Serializable]
    public class AzureTableSagaConcurrencyException :
        SagaException
    {
        public AzureTableSagaConcurrencyException()
        {
        }

        public AzureTableSagaConcurrencyException(string message, Type sagaType, Guid correlationId)
            : base(message, sagaType, correlationId)
        {
        }
    }
}

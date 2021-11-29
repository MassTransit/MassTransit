namespace MassTransit
{
    using System;


    [Serializable]
    public class ConcurrencyException :
        SagaException
    {
        public ConcurrencyException()
        {
        }

        public ConcurrencyException(string message, Type sagaType, Guid correlationId)
            : base(message, sagaType, correlationId)
        {
        }

        public ConcurrencyException(string message, Type sagaType, Guid correlationId, Exception innerException)
            : base(message, sagaType, correlationId, innerException)
        {
        }
    }
}

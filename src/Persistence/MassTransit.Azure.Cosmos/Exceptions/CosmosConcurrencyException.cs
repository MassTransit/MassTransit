namespace MassTransit
{
    using System;


    [Serializable]
    public class CosmosConcurrencyException :
        ConcurrencyException
    {
        public CosmosConcurrencyException()
        {
        }

        public CosmosConcurrencyException(string message, Type sagaType, Guid correlationId)
            : base(message, sagaType, correlationId)
        {
        }

        public CosmosConcurrencyException(string message, Type sagaType, Guid correlationId, Exception innerException)
            : base(message, sagaType, correlationId, innerException)
        {
        }
    }
}

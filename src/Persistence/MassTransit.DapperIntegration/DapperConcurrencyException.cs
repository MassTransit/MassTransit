namespace MassTransit
{
    using System;


    [Serializable]
    public class DapperConcurrencyException :
        ConcurrencyException
    {
        public DapperConcurrencyException(string message, Type sagaType, Guid correlationId)
            : base(message, sagaType, correlationId)
        {
        }

        public DapperConcurrencyException(string message, Type sagaType, Guid correlationId, Exception innerException)
            : base(message, sagaType, correlationId, innerException)
        {
        }

        public DapperConcurrencyException()
        {
        }
    }
}

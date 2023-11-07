namespace MassTransit.Exceptions
{
    using System;


    [Serializable]
    public class CassandraDbSagaConcurrencyException :
        ConcurrencyException
    {
        public CassandraDbSagaConcurrencyException(string message, Type sagaType, Guid correlationId)
            : base(message, sagaType, correlationId)
        {
        }

        public CassandraDbSagaConcurrencyException(string message, Type sagaType, Guid correlationId, Exception innerException)
            : base(message, sagaType, correlationId, innerException)
        {
        }

        public CassandraDbSagaConcurrencyException()
        {
        }
    }
}

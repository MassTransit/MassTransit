using System;
using System.Runtime.Serialization;

namespace MassTransit.EventStoreDbIntegration.Exceptions
{
    [Serializable]
    public class EventStoreDbConnectionException :
        ConnectionException
    {
        public EventStoreDbConnectionException()
        {
        }

        public EventStoreDbConnectionException(string message)
            : base(message)
        {
        }

        public EventStoreDbConnectionException(string message, Exception innerException)
            : base(message, innerException, IsExceptionTransient(innerException))
        {
        }

        protected EventStoreDbConnectionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        static bool IsExceptionTransient(Exception exception)
        {
            return exception switch
            {
                UnauthorizedAccessException _ => false,
                _ => true
            };
        }
    }
}

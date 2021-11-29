namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class EventHubConnectionException :
        ConnectionException
    {
        public EventHubConnectionException()
        {
        }

        public EventHubConnectionException(string message)
            : base(message)
        {
        }

        public EventHubConnectionException(string message, Exception innerException)
            : base(message, innerException, IsExceptionTransient(innerException))
        {
        }

        protected EventHubConnectionException(SerializationInfo info, StreamingContext context)
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

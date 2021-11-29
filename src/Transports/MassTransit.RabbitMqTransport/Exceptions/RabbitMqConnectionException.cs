namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;
    using RabbitMQ.Client.Exceptions;


    [Serializable]
    public class RabbitMqConnectionException :
        ConnectionException
    {
        public RabbitMqConnectionException()
        {
        }

        public RabbitMqConnectionException(string message)
            : base(message)
        {
        }

        public RabbitMqConnectionException(string message, Exception innerException)
            : base(message, innerException, IsExceptionTransient(innerException))
        {
        }

        protected RabbitMqConnectionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        static bool IsExceptionTransient(Exception exception)
        {
            return exception switch
            {
                BrokerUnreachableException bue => bue.InnerException switch
                {
                    AuthenticationFailureException _ => false,
                    _ => true
                },
                _ => true
            };
        }
    }
}

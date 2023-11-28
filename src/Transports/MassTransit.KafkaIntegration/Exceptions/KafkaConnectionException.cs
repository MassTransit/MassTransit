namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;
    using Confluent.Kafka;


    [Serializable]
    public class KafkaConnectionException :
        ConnectionException
    {
        public KafkaConnectionException()
        {
        }

        public KafkaConnectionException(string message)
            : base(message)
        {
        }

        public KafkaConnectionException(string message, Exception innerException)
            : base(message, innerException, IsExceptionTransient(innerException))
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected KafkaConnectionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        static bool IsExceptionTransient(Exception exception)
        {
            return exception switch
            {
                KafkaException bue => bue.Error.IsLocalError,
                _ => true
            };
        }
    }
}

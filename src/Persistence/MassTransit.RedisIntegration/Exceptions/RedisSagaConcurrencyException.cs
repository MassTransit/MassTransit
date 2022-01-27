namespace MassTransit
{
    using System;


    [Serializable]
    public class RedisSagaConcurrencyException :
        ConcurrencyException
    {
        public RedisSagaConcurrencyException(string message, Type sagaType, Guid correlationId)
            : base(message, sagaType, correlationId)
        {
        }

        public RedisSagaConcurrencyException(string message, Type sagaType, Guid correlationId, Exception innerException)
            : base(message, sagaType, correlationId, innerException)
        {
        }

        public RedisSagaConcurrencyException()
        {
        }
    }
}

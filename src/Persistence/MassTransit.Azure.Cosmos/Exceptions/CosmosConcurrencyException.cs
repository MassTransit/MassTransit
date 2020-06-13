namespace MassTransit.Azure.Cosmos
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class CosmosConcurrencyException :
        MassTransitException
    {
        public CosmosConcurrencyException()
        {
        }

        public CosmosConcurrencyException(string message)
            : base(message)
        {
        }

        public CosmosConcurrencyException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public CosmosConcurrencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

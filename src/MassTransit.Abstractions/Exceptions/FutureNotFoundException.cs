namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class FutureNotFoundException :
        MassTransitException
    {
        public FutureNotFoundException()
        {
        }

        public FutureNotFoundException(Type type, Guid id)
            : base($"Future {TypeCache.GetShortName(type)}({id}) not found")
        {
        }

        public FutureNotFoundException(string message)
            : base(message)
        {
        }

        public FutureNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected FutureNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

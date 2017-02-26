namespace MassTransit.Util.Caching
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class ValueFactoryException :
        Exception
    {
        public ValueFactoryException()
        {
        }

        protected ValueFactoryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ValueFactoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
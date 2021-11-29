namespace MassTransit
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

        public ValueFactoryException(string message)
            : base(message)
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

namespace MassTransit.Internals.Caching
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

    #if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
    #endif
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

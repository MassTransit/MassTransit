namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class TransportException :
        AbstractUriException
    {
        public TransportException()
        {
        }

        public TransportException(Uri uri)
            : base(uri)
        {
        }

        public TransportException(Uri uri, string message)
            : base(uri, message)
        {
        }

        public TransportException(Uri uri, string message, Exception innerException)
            : base(uri, message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected TransportException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

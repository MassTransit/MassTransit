namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class PayloadFactoryException :
        PayloadException
    {
        public PayloadFactoryException()
        {
        }

        public PayloadFactoryException(string message)
            : base(message)
        {
        }

        public PayloadFactoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected PayloadFactoryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

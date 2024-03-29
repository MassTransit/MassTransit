namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class MessageInitializerException :
        MassTransitException
    {
        public MessageInitializerException()
        {
        }

        public MessageInitializerException(string messageType, string propertyName, string propertType, string message)
            : base($"The {messageType} message initializer for property {propertyName}({propertType}) failed: {message}")
        {
        }

        public MessageInitializerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected MessageInitializerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

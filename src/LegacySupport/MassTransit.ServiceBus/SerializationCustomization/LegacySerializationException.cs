namespace MassTransit.LegacySupport.SerializationCustomization
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class LegacySerializationException :
        Exception
    {
        public LegacySerializationException()
        {
        }

        public LegacySerializationException(string message) : base(message)
        {
        }

        public LegacySerializationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LegacySerializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
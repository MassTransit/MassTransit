namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class RoutingSlipArgumentException :
        RoutingSlipException
    {
        public RoutingSlipArgumentException()
        {
        }

        public RoutingSlipArgumentException(string message)
            : base(message)
        {
        }

        public RoutingSlipArgumentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected RoutingSlipArgumentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

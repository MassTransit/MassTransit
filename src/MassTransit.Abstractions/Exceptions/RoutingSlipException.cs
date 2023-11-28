namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class RoutingSlipException :
        CourierException
    {
        public RoutingSlipException()
        {
        }

        public RoutingSlipException(string message)
            : base(message)
        {
        }

        public RoutingSlipException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected RoutingSlipException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

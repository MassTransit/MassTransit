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

        protected RoutingSlipException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

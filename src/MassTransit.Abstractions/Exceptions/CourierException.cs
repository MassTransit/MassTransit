namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class CourierException :
        MassTransitException
    {
        public CourierException()
        {
        }

        public CourierException(string message)
            : base(message)
        {
        }

        public CourierException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CourierException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

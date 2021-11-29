namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class ActivityCompensationException :
        CourierException
    {
        public ActivityCompensationException()
        {
        }

        public ActivityCompensationException(string message)
            : base(message)
        {
        }

        public ActivityCompensationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ActivityCompensationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

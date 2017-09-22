namespace MassTransit.Courier.MongoDbIntegration
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class SaveEventException :
        Exception
    {
        public SaveEventException()
        {
        }
        public Guid TrackingNumber { get; private set; }

        public SaveEventException(Guid trackingNumber, string message)
            : base(message)
        {
            TrackingNumber = trackingNumber;
        }

        public SaveEventException(Guid trackingNumber, string message, Exception innerException)
            : base(message, innerException)
        {
            TrackingNumber = trackingNumber;
        }
        
        protected SaveEventException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            TrackingNumber = (Guid)info.GetValue("TrackingNumber", typeof(Guid));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("TrackingNumber", TrackingNumber);
        }
    }
}
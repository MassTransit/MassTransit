namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class MongoDbSaveEventException :
        Exception
    {
        public MongoDbSaveEventException()
        {
        }

        public MongoDbSaveEventException(Guid trackingNumber, string message)
            : base(message)
        {
            TrackingNumber = trackingNumber;
        }

        public MongoDbSaveEventException(Guid trackingNumber, string message, Exception innerException)
            : base(message, innerException)
        {
            TrackingNumber = trackingNumber;
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected MongoDbSaveEventException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            TrackingNumber = (Guid)info.GetValue("TrackingNumber", typeof(Guid));
        }

        public Guid TrackingNumber { get; private set; }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("TrackingNumber", TrackingNumber);
        }
    }
}

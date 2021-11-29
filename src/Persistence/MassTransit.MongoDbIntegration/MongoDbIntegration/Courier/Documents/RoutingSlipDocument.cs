namespace MassTransit.MongoDbIntegration.Courier.Documents
{
    using System;
    using Events;


    public class RoutingSlipDocument
    {
        public RoutingSlipDocument(Guid trackingNumber)
        {
            TrackingNumber = trackingNumber;
        }

        public Guid TrackingNumber { get; private set; }
        public RoutingSlipEventDocument[] Events { get; private set; }
    }
}

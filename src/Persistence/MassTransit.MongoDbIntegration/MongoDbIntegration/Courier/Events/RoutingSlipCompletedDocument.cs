namespace MassTransit.MongoDbIntegration.Courier.Events
{
    using System.Collections.Generic;
    using MassTransit.Courier.Contracts;


    public class RoutingSlipCompletedDocument :
        RoutingSlipEventDocument
    {
        public RoutingSlipCompletedDocument(RoutingSlipCompleted message)
            : base(message.Timestamp, message.Duration)
        {
            Variables = message.Variables;
        }

        public IDictionary<string, object> Variables { get; private set; }
    }
}

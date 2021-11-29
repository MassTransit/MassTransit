namespace MassTransit.MongoDbIntegration.Courier.Events
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Courier.Contracts;


    public class RoutingSlipActivityCompletedDocument :
        RoutingSlipEventDocument
    {
        public RoutingSlipActivityCompletedDocument(RoutingSlipActivityCompleted message)
            : base(message.Timestamp, message.Duration, message.Host)
        {
            ActivityName = message.ActivityName;
            ExecutionId = message.ExecutionId;
            Data = message.Data;
            Variables = message.Variables;
        }

        public string ActivityName { get; private set; }
        public Guid ExecutionId { get; private set; }
        public IDictionary<string, object> Data { get; private set; }
        public IDictionary<string, object> Variables { get; private set; }
    }
}

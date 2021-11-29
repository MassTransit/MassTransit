namespace MassTransit.MongoDbIntegration.Courier.Events
{
    using System.Linq;
    using Documents;
    using MassTransit.Courier.Contracts;


    public class RoutingSlipFaultedDocument :
        RoutingSlipEventDocument
    {
        public RoutingSlipFaultedDocument(RoutingSlipFaulted message)
            : base(message.Timestamp, message.Duration)
        {
            if (message.ActivityExceptions != null)
                ActivityExceptions = message.ActivityExceptions.Select(x => new ActivityExceptionDocument(x)).ToArray();
        }

        public ActivityExceptionDocument[] ActivityExceptions { get; private set; }
    }
}

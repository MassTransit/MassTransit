namespace MassTransit.MongoDbIntegration.Courier.Events
{
    using Documents;
    using MassTransit.Courier.Contracts;


    public class RoutingSlipCompensationFailedDocument :
        RoutingSlipEventDocument
    {
        public RoutingSlipCompensationFailedDocument(RoutingSlipCompensationFailed message)
            : base(message.Timestamp, message.Duration)
        {
            if (message.ExceptionInfo != null)
                ExceptionInfo = new ExceptionInfoDocument(message.ExceptionInfo);
        }

        public ExceptionInfoDocument ExceptionInfo { get; private set; }
    }
}

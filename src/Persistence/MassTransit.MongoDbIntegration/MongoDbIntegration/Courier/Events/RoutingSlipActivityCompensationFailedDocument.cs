namespace MassTransit.MongoDbIntegration.Courier.Events
{
    using System;
    using Documents;
    using MassTransit.Courier.Contracts;


    public class RoutingSlipActivityCompensationFailedDocument :
        RoutingSlipEventDocument
    {
        public RoutingSlipActivityCompensationFailedDocument(RoutingSlipActivityCompensationFailed message)
            : base(message.Timestamp, message.Duration, message.Host)
        {
            ActivityName = message.ActivityName;
            ExecutionId = message.ExecutionId;

            if (message.ExceptionInfo != null)
                ExceptionInfo = new ExceptionInfoDocument(message.ExceptionInfo);
        }

        public string ActivityName { get; private set; }
        public Guid ExecutionId { get; private set; }
        public ExceptionInfoDocument ExceptionInfo { get; private set; }
    }
}

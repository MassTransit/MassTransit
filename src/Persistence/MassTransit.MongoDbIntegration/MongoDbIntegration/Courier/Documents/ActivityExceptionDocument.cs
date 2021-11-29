namespace MassTransit.MongoDbIntegration.Courier.Documents
{
    using System;
    using MassTransit.Courier.Contracts;


    public class ActivityExceptionDocument
    {
        public ActivityExceptionDocument(ActivityException activityException)
        {
            ExecutionId = activityException.ExecutionId;
            ActivityName = activityException.Name;
            Timestamp = activityException.Timestamp;

            Host = new HostDocument(activityException.Host);

            if (activityException.ExceptionInfo != null)
                ExceptionInfo = new ExceptionInfoDocument(activityException.ExceptionInfo);
        }

        public string ActivityName { get; private set; }
        public Guid ExecutionId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public HostDocument Host { get; private set; }
        public ExceptionInfoDocument ExceptionInfo { get; private set; }
    }
}

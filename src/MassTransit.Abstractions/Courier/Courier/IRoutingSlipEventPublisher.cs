namespace MassTransit.Courier
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Contracts;


    public interface IRoutingSlipEventPublisher
    {
        Task PublishRoutingSlipCompleted(DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables);

        Task PublishRoutingSlipFaulted(DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables,
            params ActivityException[] exceptions);

        Task PublishRoutingSlipActivityCompleted(string activityName, Guid executionId,
            DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables, IDictionary<string, object> arguments,
            IDictionary<string, object> data);

        Task PublishRoutingSlipActivityFaulted(string activityName, Guid executionId, DateTime timestamp, TimeSpan duration, ExceptionInfo exceptionInfo,
            IDictionary<string, object> variables, IDictionary<string, object> arguments);

        Task PublishRoutingSlipActivityCompensationFailed(string activityName, Guid executionId,
            DateTime timestamp, TimeSpan duration, DateTime failureTimestamp, TimeSpan routingSlipDuration,
            ExceptionInfo exceptionInfo, IDictionary<string, object> variables, IDictionary<string, object> data);

        Task PublishRoutingSlipActivityCompensated(string activityName, Guid executionId, DateTime timestamp, TimeSpan duration,
            IDictionary<string, object> variables, IDictionary<string, object> data);

        Task PublishRoutingSlipRevised(string activityName, Guid executionId, DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables,
            IList<Activity> itinerary,
            IList<Activity> previousItinerary);

        Task PublishRoutingSlipTerminated(string activityName, Guid executionId, DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables,
            IList<Activity> previousItinerary);
    }
}

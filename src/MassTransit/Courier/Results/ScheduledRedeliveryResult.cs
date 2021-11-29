namespace MassTransit.Courier.Results
{
    using System;
    using System.Threading.Tasks;


    public class ScheduledRedeliveryResult :
        ExecutionResult,
        CompensationResult
    {
        public bool IsFailed(out Exception exception)
        {
            exception = default;
            return false;
        }

        public Task Evaluate()
        {
            return Task.CompletedTask;
        }

        public bool IsFaulted(out Exception exception)
        {
            exception = default;
            return false;
        }
    }
}

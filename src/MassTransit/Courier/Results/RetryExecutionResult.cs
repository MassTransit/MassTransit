namespace MassTransit.Courier.Results
{
    using System;
    using System.Threading.Tasks;
    using Util;


    class RetryExecutionResult :
        ExecutionResult
    {
        public Task Evaluate()
        {
            return TaskUtil.Completed;
        }

        public bool IsFaulted(out Exception exception)
        {
            exception = null;
            return false;
        }
    }
}

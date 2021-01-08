namespace MassTransit.Courier.Results
{
    using System;
    using System.Threading.Tasks;
    using Util;


    class RetryCompensationResult :
        CompensationResult
    {
        public Task Evaluate()
        {
            return TaskUtil.Completed;
        }

        public bool IsFailed(out Exception exception)
        {
            exception = null;
            return false;
        }
    }
}

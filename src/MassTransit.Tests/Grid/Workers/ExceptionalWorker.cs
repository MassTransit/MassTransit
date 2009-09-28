namespace MassTransit.Tests.Grid
{
    using System;
    using Parallel;

	public class ExceptionalWorker :
        ISubTaskWorker<FactorLongNumber, LongNumberFactored>
    {
        public void ExecuteTask(FactorLongNumber task, Action<LongNumberFactored> result)
        {
            throw new System.NotImplementedException();
        }
    }
}
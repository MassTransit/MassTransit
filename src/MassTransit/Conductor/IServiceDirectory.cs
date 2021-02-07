namespace MassTransit.Conductor
{
    using System;
    using Directory;
    using Orchestration;


    public interface IServiceDirectory
    {
        IOrchestrationPlanner<TResult> GetOrchestrationPlanner<TResult>(params Type[] inputTypes)
            where TResult : class;
    }
}

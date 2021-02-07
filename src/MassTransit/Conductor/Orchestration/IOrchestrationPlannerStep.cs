namespace MassTransit.Conductor.Orchestration
{
    using System;


    public interface IOrchestrationPlannerStep<TInput, T, TResult> :
        IOrchestrationPlanStep<TResult>
        where TInput : class
        where T : class
        where TResult : class
    {
        void Build(BuildContext<TInput, TResult> context, IOrchestrationPlan<T, TResult> next);
    }


    public interface IOrchestrationPlanStep<TResult>
        where TResult : class
    {
        Type ServiceType { get; }
        Type RequestType { get; }
        Uri InputAddress { get; }
    }
}

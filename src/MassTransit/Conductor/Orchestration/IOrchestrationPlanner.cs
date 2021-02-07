namespace MassTransit.Conductor.Orchestration
{
    public interface IOrchestrationPlanner<TResult>
        where TResult : class
    {
        IOrchestrationPlan<TInput, TResult> BuildExecutionPlan<TInput>()
            where TInput : class;
    }
}

namespace MassTransit.Conductor.Orchestration
{
    public interface IOrchestrationPlan<TInput, TResult>
        where TInput : class
        where TResult : class
    {
        void Build(BuildContext<TInput, TResult> context);
    }
}

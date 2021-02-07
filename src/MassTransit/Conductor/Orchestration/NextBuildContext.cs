namespace MassTransit.Conductor.Orchestration
{
    public interface NextBuildContext<TRequest, TResponse, TLeft> :
        BuildContext<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
        where TLeft : class
    {
        IOrchestration<TLeft, TResponse> BuildPath(IOrchestrationStep<TLeft, TRequest, TResponse> step);
    }
}

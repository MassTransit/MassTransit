namespace MassTransit.Agents
{
    /// <summary>
    /// An active use of a pipe context as an agent.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IActivePipeContextAgent<TContext> :
        ActivePipeContextHandle<TContext>,
        IAgent
        where TContext : class, PipeContext
    {
    }
}

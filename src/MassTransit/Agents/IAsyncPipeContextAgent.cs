namespace MassTransit.Agents
{
    public interface IAsyncPipeContextAgent<TContext> :
        IAsyncPipeContextHandle<TContext>,
        IPipeContextAgent<TContext>
        where TContext : class, PipeContext
    {
    }
}

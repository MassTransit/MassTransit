namespace MassTransit.Middleware
{
    public interface IOutputPipeFilter<TInput, out TOutput> :
        IFilter<TInput>,
        IPipeConnector<TOutput>,
        IFilterObserverConnector<TOutput>
        where TInput : class, PipeContext
        where TOutput : class, PipeContext
    {
    }


    public interface IOutputPipeFilter<TInput, out TOutput, in TKey> :
        IOutputPipeFilter<TInput, TOutput>,
        IKeyPipeConnector<TKey>
        where TInput : class, PipeContext
        where TOutput : class, PipeContext
    {
    }
}

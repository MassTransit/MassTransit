namespace MassTransit
{
    public delegate TInput MergeFilterContextProvider<TInput, in TSplit>(TInput inputContext, TSplit context)
        where TSplit : class, PipeContext
        where TInput : class, PipeContext;
}

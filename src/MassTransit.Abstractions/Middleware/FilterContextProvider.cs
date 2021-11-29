namespace MassTransit
{
    public delegate TInput FilterContextProvider<out TInput, in TSplit>(TSplit context)
        where TSplit : class, PipeContext
        where TInput : class, PipeContext;
}

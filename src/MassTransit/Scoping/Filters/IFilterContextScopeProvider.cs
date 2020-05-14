namespace MassTransit.Scoping.Filters
{
    using GreenPipes;


    public interface IFilterContextScopeProvider<TContext>
        where TContext : class, PipeContext
    {
        IFilterContextScope<TContext> Create(TContext context);
    }
}

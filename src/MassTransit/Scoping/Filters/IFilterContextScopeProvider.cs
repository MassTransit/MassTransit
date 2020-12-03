namespace MassTransit.Scoping.Filters
{
    using GreenPipes;


    public interface IFilterContextScopeProvider<TContext> :
        IProbeSite
        where TContext : class, PipeContext
    {
        IFilterContextScope<TContext> Create(TContext context);
    }
}

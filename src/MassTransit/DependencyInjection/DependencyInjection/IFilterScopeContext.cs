namespace MassTransit.DependencyInjection
{
    using System;


    public interface IFilterScopeContext<TContext> :
        IAsyncDisposable
        where TContext : class, PipeContext
    {
        IFilter<TContext> Filter { get; }
        TContext Context { get; }
    }
}

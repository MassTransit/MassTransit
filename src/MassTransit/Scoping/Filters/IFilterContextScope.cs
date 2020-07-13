namespace MassTransit.Scoping.Filters
{
    using System;
    using GreenPipes;


    public interface IFilterContextScope<TContext> :
        IAsyncDisposable
        where TContext : class, PipeContext
    {
        IFilter<TContext> Filter { get; }
        TContext Context { get; }
    }
}

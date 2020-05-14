namespace MassTransit.Scoping.Filters
{
    using System;
    using GreenPipes;


    public interface IFilterContextScope<TContext> :
        IDisposable
        where TContext : class, PipeContext
    {
        IFilter<TContext> Filter { get; }
        TContext Context { get; }
    }
}

namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;
#if NET6_0_OR_GREATER
    using System;
    using System.Runtime.InteropServices;
#endif


    public class PipeBuilder<TContext> :
        IPipeBuilder<TContext>
        where TContext : class, PipeContext
    {
        readonly List<IFilter<TContext>> _filters;

        public PipeBuilder(int capacity = 4)
        {
            _filters = new List<IFilter<TContext>>(capacity);
        }

        public PipeBuilder(params IFilter<TContext>[] filters)
        {
            _filters = new List<IFilter<TContext>>(filters);
        }

        public void AddFilter(IFilter<TContext> filter)
        {
            _filters.Add(filter);
        }

    #if NET6_0_OR_GREATER
        public IPipe<TContext> Build()
        {
            Span<IFilter<TContext>> items = CollectionsMarshal.AsSpan(_filters);
            if (items.Length == 0)
                return Pipe.Empty<TContext>();

            IPipe<TContext> current = new LastPipe<TContext>(items[items.Length - 1]);

            for (var i = items.Length - 2; i >= 0; i--)
                current = new FilterPipe<TContext>(items[i], current);

            return current;
        }
    #else
        public IPipe<TContext> Build()
        {
            if (_filters.Count == 0)
                return Pipe.Empty<TContext>();

            IPipe<TContext> current = new LastPipe<TContext>(_filters[_filters.Count - 1]);

            for (var i = _filters.Count - 2; i >= 0; i--)
                current = new FilterPipe<TContext>(_filters[i], current);

            return current;
        }
    #endif
    }
}

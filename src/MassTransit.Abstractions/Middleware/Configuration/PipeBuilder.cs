namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


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

        public IPipe<TContext> Build()
        {
            if (_filters.Count == 0)
                return Pipe.Empty<TContext>();

            IPipe<TContext> current = new LastPipe<TContext>(_filters[_filters.Count - 1]);

            for (var i = _filters.Count - 2; i >= 0; i--)
                current = new FilterPipe<TContext>(_filters[i], current);

            return current;
        }
    }
}

namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


    public class SpecificationPipeBuilder<TContext> :
        ISpecificationPipeBuilder<TContext>
        where TContext : class, PipeContext
    {
        readonly List<IFilter<TContext>> _filters;

        public SpecificationPipeBuilder()
        {
            _filters = new List<IFilter<TContext>>();
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

        public IPipe<TContext> Build(IPipe<TContext> lastPipe)
        {
            if (_filters.Count == 0)
                return lastPipe;

            IPipe<TContext> current = lastPipe;

            for (var i = _filters.Count - 1; i >= 0; i--)
                current = new FilterPipe<TContext>(_filters[i], current);

            return current;
        }

        public bool IsDelegated => false;
        public bool IsImplemented => false;

        public ISpecificationPipeBuilder<TContext> CreateDelegatedBuilder()
        {
            return new ChildSpecificationPipeBuilder<TContext>(this, IsImplemented, true);
        }

        public ISpecificationPipeBuilder<TContext> CreateImplementedBuilder()
        {
            return new ChildSpecificationPipeBuilder<TContext>(this, true, IsDelegated);
        }
    }
}

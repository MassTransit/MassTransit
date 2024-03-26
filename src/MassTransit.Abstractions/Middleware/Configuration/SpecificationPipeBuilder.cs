namespace MassTransit.Configuration
{
    using System.Collections.Generic;


    public partial class PipeConfigurator<TContext>
        where TContext : class, PipeContext
    {
        public class SpecificationPipeBuilder :
            ISpecificationPipeBuilder<TContext>
        {
            readonly List<IFilter<TContext>> _filters;

            public SpecificationPipeBuilder()
            {
                _filters = new List<IFilter<TContext>>(16);
            }

            public void AddFilter(IFilter<TContext> filter)
            {
                _filters.Add(filter);
            }

            public bool IsDelegated => false;
            public bool IsImplemented => false;

            public ISpecificationPipeBuilder<TContext> CreateDelegatedBuilder()
            {
                return new ChildSpecificationPipeBuilder(this, IsImplemented, true);
            }

            public ISpecificationPipeBuilder<TContext> CreateImplementedBuilder()
            {
                return new ChildSpecificationPipeBuilder(this, true, IsDelegated);
            }

            public IPipe<TContext> Build()
            {
                if (_filters.Count == 0)
                    return Cache.EmptyPipe;

                IPipe<TContext> current = new LastPipe(_filters[_filters.Count - 1]);

                for (var i = _filters.Count - 2; i >= 0; i--)
                    current = new FilterPipe(_filters[i], current);

                return current;
            }

            public IPipe<TContext> Build(IPipe<TContext> lastPipe)
            {
                if (_filters.Count == 0)
                    return lastPipe;

                IPipe<TContext> current = lastPipe;

                for (var i = _filters.Count - 1; i >= 0; i--)
                    current = new FilterPipe(_filters[i], current);

                return current;
            }
        }
    }
}

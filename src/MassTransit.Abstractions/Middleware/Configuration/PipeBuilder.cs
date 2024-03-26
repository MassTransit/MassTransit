namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;


    public partial class PipeConfigurator<TContext>
        where TContext : class, PipeContext
    {
        public class PipeBuilder :
            IPipeBuilder<TContext>
        {
            readonly List<IFilter<TContext>> _filters;

            public PipeBuilder(int capacity = 16)
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

            public void Method1()
            {
            }

            public void Method2()
            {
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
        }


        internal static class Cache
        {
            internal static readonly IPipe<TContext> EmptyPipe = new EmptyPipe();
            internal static readonly IPipe<TContext> LastPipe = new Last();
        }


        public class EmptyPipe :
            IPipe<TContext>
        {
            [DebuggerNonUserCode]
            Task IPipe<TContext>.Send(TContext context)
            {
                return Task.CompletedTask;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
            }
        }


        public class FilterPipe :
            IPipe<TContext>
        {
            readonly IFilter<TContext> _filter;
            readonly IPipe<TContext> _next;

            public FilterPipe(IFilter<TContext> filter, IPipe<TContext> next)
            {
                _filter = filter;
                _next = next;
            }

            public void Probe(ProbeContext context)
            {
                _filter.Probe(context);
                _next.Probe(context);
            }

            [DebuggerStepThrough]
            public Task Send(TContext context)
            {
                return _filter.Send(context, _next);
            }
        }


        /// <summary>
        /// The last pipe in a pipeline is always an end pipe that does nothing and returns synchronously
        /// </summary>
        public class LastPipe :
            IPipe<TContext>
        {
            readonly IFilter<TContext> _filter;

            public LastPipe(IFilter<TContext> filter)
            {
                _filter = filter;
            }

            public void Probe(ProbeContext context)
            {
                _filter.Probe(context);
            }

            [DebuggerStepThrough]
            public Task Send(TContext context)
            {
                return _filter.Send(context, Cache.LastPipe);
            }
        }


        class Last :
            IPipe<TContext>
        {
            public void Probe(ProbeContext context)
            {
            }

            public Task Send(TContext context)
            {
                return Task.CompletedTask;
            }
        }
    }
}

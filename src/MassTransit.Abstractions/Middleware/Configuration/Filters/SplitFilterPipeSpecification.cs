namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


    /// <summary>
    /// Adds an arbitrary filter to the pipe
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TFilter">The filter type</typeparam>
    public class SplitFilterPipeSpecification<TContext, TFilter> :
        IPipeSpecification<TContext>
        where TContext : class, PipeContext
        where TFilter : class, PipeContext
    {
        readonly MergeFilterContextProvider<TContext, TFilter> _contextProvider;
        readonly FilterContextProvider<TFilter, TContext> _inputContextProvider;
        readonly IPipeSpecification<TFilter> _specification;

        public SplitFilterPipeSpecification(IPipeSpecification<TFilter> specification, MergeFilterContextProvider<TContext, TFilter> contextProvider,
            FilterContextProvider<TFilter, TContext> inputContextProvider)
        {
            _specification = specification;
            _contextProvider = contextProvider;
            _inputContextProvider = inputContextProvider;
        }

        public void Apply(IPipeBuilder<TContext> builder)
        {
            var splitBuilder = new Builder(builder, _contextProvider, _inputContextProvider);

            _specification.Apply(splitBuilder);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_specification == null)
                yield return this.Failure("Specification", "must not be null");
            if (_contextProvider == null)
                yield return this.Failure("ContextProvider", "must not be null");
        }


        class Builder :
            IPipeBuilder<TFilter>
        {
            readonly IPipeBuilder<TContext> _builder;
            readonly MergeFilterContextProvider<TContext, TFilter> _contextProvider;
            readonly FilterContextProvider<TFilter, TContext> _inputContextProvider;

            public Builder(IPipeBuilder<TContext> builder, MergeFilterContextProvider<TContext, TFilter> contextProvider,
                FilterContextProvider<TFilter, TContext> inputContextProvider)
            {
                _builder = builder;
                _contextProvider = contextProvider;
                _inputContextProvider = inputContextProvider;
            }

            public void AddFilter(IFilter<TFilter> filter)
            {
                var splitFilter = new SplitFilter<TContext, TFilter>(filter, _contextProvider, _inputContextProvider);

                _builder.AddFilter(splitFilter);
            }
        }
    }
}

namespace MassTransit.Configuration
{
    using System.Collections.Generic;


    /// <summary>
    /// Adds an arbitrary filter to the pipe
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class FilterPipeSpecification<TContext> :
        IPipeSpecification<TContext>
        where TContext : class, PipeContext
    {
        readonly IFilter<TContext> _filter;

        public FilterPipeSpecification(IFilter<TContext> filter)
        {
            _filter = filter;
        }

        public void Apply(IPipeBuilder<TContext> builder)
        {
            builder.AddFilter(_filter);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_filter == null)
                yield return this.Failure("Filter", "must not be null");
        }
    }
}

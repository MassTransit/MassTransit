namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


    /// <summary>
    /// Adds an arbitrary filter to the pipe
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class InlineFilterPipeSpecification<TContext> :
        IPipeSpecification<TContext>
        where TContext : class, PipeContext
    {
        readonly InlineFilterMethod<TContext> _filterMethod;

        public InlineFilterPipeSpecification(InlineFilterMethod<TContext> filterMethod)
        {
            _filterMethod = filterMethod;
        }

        public void Apply(IPipeBuilder<TContext> builder)
        {
            builder.AddFilter(new InlineFilter<TContext>(_filterMethod));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_filterMethod == null)
                yield return this.Failure("FilterMethod", "must not be null");
        }
    }
}

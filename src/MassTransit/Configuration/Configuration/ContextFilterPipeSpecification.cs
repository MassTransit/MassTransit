namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Middleware;


    public class ContextFilterPipeSpecification<TContext> :
        IPipeSpecification<TContext>
        where TContext : class, PipeContext
    {
        readonly Func<TContext, Task<bool>> _filter;

        public ContextFilterPipeSpecification(Func<TContext, Task<bool>> filter)
        {
            _filter = filter;
        }

        public void Apply(IPipeBuilder<TContext> builder)
        {
            builder.AddFilter(new ContextFilter<TContext>(_filter));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_filter == null)
                yield return this.Failure("Filter", "must not be null");
        }
    }
}

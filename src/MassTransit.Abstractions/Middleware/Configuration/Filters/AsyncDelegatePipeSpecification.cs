namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Middleware;


    public class AsyncDelegatePipeSpecification<T> :
        IPipeSpecification<T>
        where T : class, PipeContext
    {
        readonly Func<T, Task> _callback;

        public AsyncDelegatePipeSpecification(Func<T, Task> callback)
        {
            _callback = callback;
        }

        public void Apply(IPipeBuilder<T> builder)
        {
            builder.AddFilter(new AsyncDelegateFilter<T>(_callback));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_callback == null)
                yield return this.Failure("Callback", "must not be null");
        }
    }
}

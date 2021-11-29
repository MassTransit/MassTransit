namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Middleware;


    public class DelegatePipeSpecification<T> :
        IPipeSpecification<T>
        where T : class, PipeContext
    {
        readonly Action<T> _callback;

        public DelegatePipeSpecification(Action<T> callback)
        {
            _callback = callback;
        }

        public void Apply(IPipeBuilder<T> builder)
        {
            builder.AddFilter(new DelegateFilter<T>(_callback));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_callback == null)
                yield return this.Failure("Callback", "must not be null");
        }
    }
}

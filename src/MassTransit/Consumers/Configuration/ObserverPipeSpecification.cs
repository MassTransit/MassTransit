namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Middleware;


    /// <summary>
    /// Adds a message handler to the consuming pipe builder
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public class ObserverPipeSpecification<T> :
        IPipeSpecification<ConsumeContext<T>>
        where T : class
    {
        readonly IObserver<ConsumeContext<T>> _observer;

        public ObserverPipeSpecification(IObserver<ConsumeContext<T>> observer)
        {
            _observer = observer;
        }

        void IPipeSpecification<ConsumeContext<T>>.Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            builder.AddFilter(new ObserverMessageFilter<T>(_observer));
        }

        IEnumerable<ValidationResult> ISpecification.Validate()
        {
            if (_observer == null)
                yield return this.Failure("Handler", "must not be null");
        }
    }
}

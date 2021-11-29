namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


    /// <summary>
    /// Adds a message handler to the consuming pipe builder
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public class HandlerPipeSpecification<T> :
        IPipeSpecification<ConsumeContext<T>>
        where T : class
    {
        readonly MessageHandler<T> _handler;

        public HandlerPipeSpecification(MessageHandler<T> handler)
        {
            _handler = handler;
        }

        void IPipeSpecification<ConsumeContext<T>>.Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            builder.AddFilter(new HandlerMessageFilter<T>(_handler));
        }

        IEnumerable<ValidationResult> ISpecification.Validate()
        {
            if (_handler == null)
                yield return this.Failure("Handler", "must not be null");
        }
    }
}

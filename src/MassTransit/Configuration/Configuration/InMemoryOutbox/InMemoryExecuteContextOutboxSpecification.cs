namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;
    using Middleware.InMemoryOutbox;


    public class InMemoryExecuteContextOutboxSpecification<TArguments> :
        IPipeSpecification<ExecuteContext<TArguments>>,
        IOutboxConfigurator
        where TArguments : class
    {
        public bool ConcurrentMessageDelivery { get; set; }

        public void Apply(IPipeBuilder<ExecuteContext<TArguments>> builder)
        {
            builder.AddFilter(
                new InMemoryOutboxFilter<ExecuteContext<TArguments>, InMemoryOutboxExecuteContext<TArguments>>(Factory, ConcurrentMessageDelivery));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        static InMemoryOutboxExecuteContext<TArguments> Factory(ExecuteContext<TArguments> context)
        {
            return new InMemoryOutboxExecuteContext<TArguments>(context);
        }
    }
}

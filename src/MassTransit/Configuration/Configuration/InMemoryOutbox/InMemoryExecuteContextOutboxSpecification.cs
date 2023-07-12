namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Middleware;
    using Middleware.InMemoryOutbox;


    public class InMemoryExecuteContextOutboxSpecification<TArguments> :
        IPipeSpecification<ExecuteContext<TArguments>>,
        IOutboxConfigurator
        where TArguments : class
    {
        readonly ISetScopedConsumeContext _setter;

        public InMemoryExecuteContextOutboxSpecification(IRegistrationContext context)
            : this(context as ISetScopedConsumeContext ?? throw new ArgumentException(nameof(context)))
        {
        }

        public InMemoryExecuteContextOutboxSpecification(ISetScopedConsumeContext setter)
        {
            _setter = setter;
        }

        public bool ConcurrentMessageDelivery { get; set; }

        public void Apply(IPipeBuilder<ExecuteContext<TArguments>> builder)
        {
            builder.AddFilter(
                new InMemoryOutboxFilter<ExecuteContext<TArguments>, InMemoryOutboxExecuteContext<TArguments>>(_setter, Factory, ConcurrentMessageDelivery));
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

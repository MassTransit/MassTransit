namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Middleware;
    using Middleware.InMemoryOutbox;


    public class InMemoryCompensateContextOutboxSpecification<TArguments> :
        IPipeSpecification<CompensateContext<TArguments>>,
        IOutboxConfigurator
        where TArguments : class
    {
        readonly ISetScopedConsumeContext _setter;

        public InMemoryCompensateContextOutboxSpecification(IRegistrationContext context)
            : this(context as ISetScopedConsumeContext ?? throw new ArgumentException(nameof(context)))
        {
        }

        public InMemoryCompensateContextOutboxSpecification(ISetScopedConsumeContext setter)
        {
            _setter = setter;
        }

        public bool ConcurrentMessageDelivery { get; set; }

        public void Apply(IPipeBuilder<CompensateContext<TArguments>> builder)
        {
            builder.AddFilter(
                new InMemoryOutboxFilter<CompensateContext<TArguments>, InMemoryOutboxCompensateContext<TArguments>>(_setter, Factory,
                    ConcurrentMessageDelivery));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        static InMemoryOutboxCompensateContext<TArguments> Factory(CompensateContext<TArguments> context)
        {
            return new InMemoryOutboxCompensateContext<TArguments>(context);
        }
    }
}

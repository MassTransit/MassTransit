namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Middleware;
    using Middleware.InMemoryOutbox;


    public class InMemoryOutboxSpecification<T> :
        IPipeSpecification<ConsumeContext<T>>,
        IOutboxConfigurator
        where T : class
    {
        readonly ISetScopedConsumeContext _setter;

        public InMemoryOutboxSpecification(IRegistrationContext context)
            : this(context as ISetScopedConsumeContext ?? throw new ArgumentException(nameof(context)))
        {
        }

        public InMemoryOutboxSpecification(ISetScopedConsumeContext setter)
        {
            _setter = setter;
        }

        public bool ConcurrentMessageDelivery { get; set; }

        public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            builder.AddFilter(new InMemoryOutboxFilter<ConsumeContext<T>, InMemoryOutboxConsumeContext<T>>(_setter, Factory, ConcurrentMessageDelivery));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        static InMemoryOutboxConsumeContext<T> Factory(ConsumeContext<T> context)
        {
            return new InMemoryOutboxConsumeContext<T>(context);
        }


        public class Batch :
            IPipeSpecification<ConsumeContext<Batch<T>>>,
            IOutboxConfigurator
        {
            readonly ISetScopedConsumeContext _setter;

            public Batch(IRegistrationContext context)
                : this(context as ISetScopedConsumeContext ?? throw new ArgumentException(nameof(context)))
            {
            }

            public Batch(ISetScopedConsumeContext setter)
            {
                _setter = setter;
            }

            public bool ConcurrentMessageDelivery { get; set; }

            public IEnumerable<ValidationResult> Validate()
            {
                yield break;
            }

            public void Apply(IPipeBuilder<ConsumeContext<Batch<T>>> builder)
            {
                builder.AddFilter(new InMemoryOutboxFilter<ConsumeContext<Batch<T>>, InMemoryOutboxConsumeContext<T>.Batch>(_setter, BatchFactory,
                    ConcurrentMessageDelivery));
            }

            static InMemoryOutboxConsumeContext<T>.Batch BatchFactory(ConsumeContext<Batch<T>> context)
            {
                return new InMemoryOutboxConsumeContext<T>.Batch(context);
            }
        }
    }
}

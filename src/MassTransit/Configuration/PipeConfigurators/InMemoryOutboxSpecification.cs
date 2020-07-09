namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using Context;
    using GreenPipes;
    using Pipeline.Filters;


    public class InMemoryOutboxSpecification<T> :
        IPipeSpecification<ConsumeContext<T>>,
        IOutboxConfigurator
        where T : class
    {
        public bool ConcurrentMessageDelivery { get; set; }

        public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            builder.AddFilter(new InMemoryOutboxFilter<ConsumeContext<T>, InMemoryOutboxConsumeContext<T>>(Factory, ConcurrentMessageDelivery));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        static InMemoryOutboxConsumeContext<T> Factory(ConsumeContext<T> context)
        {
            return new InMemoryOutboxConsumeContext<T>(context);
        }
    }
}

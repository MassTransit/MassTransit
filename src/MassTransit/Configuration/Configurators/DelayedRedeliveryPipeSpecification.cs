namespace MassTransit.Configurators
{
    using System.Collections.Generic;
    using GreenPipes;
    using Pipeline.Filters;


    public class DelayedRedeliveryPipeSpecification<TMessage> :
        IPipeSpecification<ConsumeContext<TMessage>>
        where TMessage : class
    {
        public void Apply(IPipeBuilder<ConsumeContext<TMessage>> builder)
        {
            builder.AddFilter(new DelayedMessageRedeliveryFilter<TMessage>());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }
    }
}

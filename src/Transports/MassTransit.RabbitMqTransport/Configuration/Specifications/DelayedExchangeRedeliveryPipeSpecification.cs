namespace MassTransit.RabbitMqTransport.Specifications
{
    using System.Collections.Generic;
    using GreenPipes;
    using Pipeline;


    public class DelayedExchangeRedeliveryPipeSpecification<TMessage> :
        IPipeSpecification<ConsumeContext<TMessage>>
        where TMessage : class
    {
        public void Apply(IPipeBuilder<ConsumeContext<TMessage>> builder)
        {
            builder.AddFilter(new DelayedExchangeMessageRedeliveryFilter<TMessage>());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }
    }
}

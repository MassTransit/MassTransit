namespace MassTransit.ActiveMqTransport.Specifications
{
    using System.Collections.Generic;
    using GreenPipes;
    using Pipeline;


    public class ActiveMqRedeliveryPipeSpecification<TMessage> :
        IPipeSpecification<ConsumeContext<TMessage>>
        where TMessage : class
    {
        public void Apply(IPipeBuilder<ConsumeContext<TMessage>> builder)
        {
            builder.AddFilter(new ActiveMqMessageRedeliveryFilter<TMessage>());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }
    }
}

namespace MassTransit.RabbitMqTransport.Specifications
{
    using System.Collections.Generic;
    using GreenPipes;
    using Pipeline;


    public class DelayedExchangeMessageSchedulerSpecification :
        IPipeSpecification<ConsumeContext>
    {
        public void Apply(IPipeBuilder<ConsumeContext> builder)
        {
            builder.AddFilter(new DelayedExchangeMessageSchedulerFilter());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }
    }
}

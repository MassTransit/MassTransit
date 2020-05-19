namespace MassTransit.AmazonSqsTransport.Specifications
{
    using System.Collections.Generic;
    using GreenPipes;
    using Pipeline;


    public class DelayedMessageSchedulerSpecification :
        IPipeSpecification<ConsumeContext>
    {
        public void Apply(IPipeBuilder<ConsumeContext> builder)
        {
            builder.AddFilter(new DelayedMessageSchedulerFilter());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }
    }
}

namespace MassTransit.ActiveMqTransport.Specifications
{
    using System.Collections.Generic;
    using GreenPipes;
    using Pipeline;


    public class ActiveMqMessageSchedulerSpecification :
        IPipeSpecification<ConsumeContext>
    {
        public void Apply(IPipeBuilder<ConsumeContext> builder)
        {
            builder.AddFilter(new ActiveMqMessageSchedulerFilter());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }
    }
}

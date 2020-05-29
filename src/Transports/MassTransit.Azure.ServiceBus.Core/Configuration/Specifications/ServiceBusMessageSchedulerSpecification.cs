namespace MassTransit.Azure.ServiceBus.Core.Specifications
{
    using System.Collections.Generic;
    using GreenPipes;
    using Pipeline;


    public class ServiceBusMessageSchedulerSpecification :
        IPipeSpecification<ConsumeContext>
    {
        public void Apply(IPipeBuilder<ConsumeContext> builder)
        {
            builder.AddFilter(new ServiceBusMessageSchedulerFilter());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }
    }
}

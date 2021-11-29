namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using AzureServiceBusTransport.Middleware;


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

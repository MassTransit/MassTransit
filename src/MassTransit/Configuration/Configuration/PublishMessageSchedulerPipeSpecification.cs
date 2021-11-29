namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


    public class PublishMessageSchedulerPipeSpecification :
        IPipeSpecification<ConsumeContext>
    {
        public void Apply(IPipeBuilder<ConsumeContext> builder)
        {
            builder.AddFilter(new PublishMessageSchedulerFilter());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }
    }
}

namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using GreenPipes;
    using Pipeline.Filters;


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

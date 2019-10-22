namespace MassTransit.WindsorIntegration.Configuration
{
    using System.Collections.Generic;
    using GreenPipes;
    using Pipeline;


    public class WindsorMessageScopePipeSpecification :
        IPipeSpecification<ConsumeContext>
    {
        public void Apply(IPipeBuilder<ConsumeContext> builder)
        {
            builder.AddFilter(new WindsorMessageScopeFilter());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }
    }
}

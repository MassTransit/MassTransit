namespace MassTransit.PrometheusIntegration.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using Pipeline;


    public class PrometheusHandlerSpecification<TMessage> :
        IPipeSpecification<ConsumeContext<TMessage>>
        where TMessage : class
    {
        public void Apply(IPipeBuilder<ConsumeContext<TMessage>> builder)
        {
            builder.AddFilter(new PrometheusHandlerFilter<TMessage>());
        }

        public IEnumerable<ValidationResult> Validate() => Enumerable.Empty<ValidationResult>();
    }
}

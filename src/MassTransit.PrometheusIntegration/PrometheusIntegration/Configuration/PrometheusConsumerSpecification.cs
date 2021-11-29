namespace MassTransit.PrometheusIntegration.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Configuration;
    using Middleware;


    public class PrometheusConsumerSpecification<TConsumer, TMessage> :
        IPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>>
        where TConsumer : class
        where TMessage : class
    {
        public void Apply(IPipeBuilder<ConsumerConsumeContext<TConsumer, TMessage>> builder)
        {
            builder.AddFilter(new PrometheusConsumerFilter<TConsumer, TMessage>());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }


    public class PrometheusReceiveSpecification :
        IPipeSpecification<ReceiveContext>
    {
        public void Apply(IPipeBuilder<ReceiveContext> builder)
        {
            builder.AddFilter(new PrometheusReceiveFilter());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}

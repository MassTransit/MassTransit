namespace MassTransit.PrometheusIntegration.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using Pipeline;
    using Saga;


    public class PrometheusSagaSpecification<TSaga, TMessage> :
        IPipeSpecification<SagaConsumeContext<TSaga, TMessage>>
        where TSaga : class, ISaga
        where TMessage : class
    {
        public void Apply(IPipeBuilder<SagaConsumeContext<TSaga, TMessage>> builder)
        {
            builder.AddFilter(new PrometheusSagaFilter<TSaga, TMessage>());
        }

        public IEnumerable<ValidationResult> Validate() => Enumerable.Empty<ValidationResult>();
    }
}

namespace MassTransit.Monitoring.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Configuration;
    using Middleware;


    public class InstrumentSagaSpecification<TSaga, TMessage> :
        IPipeSpecification<SagaConsumeContext<TSaga, TMessage>>
        where TSaga : class, ISaga
        where TMessage : class
    {
        public void Apply(IPipeBuilder<SagaConsumeContext<TSaga, TMessage>> builder)
        {
            builder.AddFilter(new InstrumentSagaFilter<TSaga, TMessage>());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}

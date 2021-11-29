namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


    public class SagaFilterSpecification<TSaga, TMessage> :
        IPipeSpecification<SagaConsumeContext<TSaga, TMessage>>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly IFilter<SagaConsumeContext<TSaga>> _filter;

        public SagaFilterSpecification(IFilter<SagaConsumeContext<TSaga>> filter)
        {
            _filter = filter;
        }

        public void Apply(IPipeBuilder<SagaConsumeContext<TSaga, TMessage>> builder)
        {
            builder.AddFilter(new SagaSplitFilter<TSaga, TMessage>(_filter));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_filter == null)
                yield return this.Failure("Filter", "must not be null");
        }
    }
}

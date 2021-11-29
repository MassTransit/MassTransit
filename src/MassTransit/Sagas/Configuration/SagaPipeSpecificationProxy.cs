namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;


    public class SagaPipeSpecificationProxy<TSaga, TMessage> :
        IPipeSpecification<SagaConsumeContext<TSaga, TMessage>>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly IPipeSpecification<SagaConsumeContext<TSaga, TMessage>> _specification;

        public SagaPipeSpecificationProxy(IPipeSpecification<SagaConsumeContext<TSaga>> specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specification = new SagaSplitFilterSpecification<TSaga, TMessage>(specification);
        }

        public SagaPipeSpecificationProxy(IPipeSpecification<ConsumeContext<TMessage>> specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specification = new SagaMessageSplitFilterSpecification<TSaga, TMessage>(specification);
        }

        public void Apply(IPipeBuilder<SagaConsumeContext<TSaga, TMessage>> builder)
        {
            _specification.Apply(builder);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _specification.Validate();
        }
    }
}

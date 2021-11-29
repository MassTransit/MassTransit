namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;


    public class ConsumerPipeSpecificationProxy<TConsumer, TMessage> :
        IPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>>
        where TConsumer : class
        where TMessage : class
    {
        readonly IPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>> _specification;

        public ConsumerPipeSpecificationProxy(IPipeSpecification<ConsumerConsumeContext<TConsumer>> specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specification = new ConsumerSplitFilterSpecification<TConsumer, TMessage>(specification);
        }

        public ConsumerPipeSpecificationProxy(IPipeSpecification<ConsumeContext<TMessage>> specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specification = new ConsumerMessageSplitFilterSpecification<TConsumer, TMessage>(specification);
        }

        public void Apply(IPipeBuilder<ConsumerConsumeContext<TConsumer, TMessage>> builder)
        {
            _specification.Apply(builder);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _specification.Validate();
        }
    }
}

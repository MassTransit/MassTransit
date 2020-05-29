namespace MassTransit.SagaSpecifications
{
    using System.Collections.Generic;
    using GreenPipes;
    using Saga;
    using Saga.Pipeline.Filters;


    public class SagaSplitFilterSpecification<TSaga, TMessage> :
        IPipeSpecification<SagaConsumeContext<TSaga, TMessage>>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly IPipeSpecification<SagaConsumeContext<TSaga>> _specification;

        public SagaSplitFilterSpecification(IPipeSpecification<SagaConsumeContext<TSaga>> specification)
        {
            _specification = specification;
        }

        public void Apply(IPipeBuilder<SagaConsumeContext<TSaga, TMessage>> builder)
        {
            _specification.Apply(new BuilderProxy(builder));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            //            if (!typeof(TSaga).HasInterface<ISaga<TMessage>>())
            //                yield return this.Failure("MessageType", $"is not consumed by {TypeMetadataCache<TSaga>.ShortName}");

            foreach (var validationResult in _specification.Validate())
                yield return validationResult;
        }


        class BuilderProxy :
            IPipeBuilder<SagaConsumeContext<TSaga>>
        {
            readonly IPipeBuilder<SagaConsumeContext<TSaga, TMessage>> _builder;

            public BuilderProxy(IPipeBuilder<SagaConsumeContext<TSaga, TMessage>> builder)
            {
                _builder = builder;
            }

            public void AddFilter(IFilter<SagaConsumeContext<TSaga>> filter)
            {
                _builder.AddFilter(new SagaSplitFilter<TSaga, TMessage>(filter));
            }
        }
    }
}

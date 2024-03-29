namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


    public partial class SagaConnector<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        public class SagaMessageSplitFilterSpecification :
            IPipeSpecification<SagaConsumeContext<TSaga, TMessage>>
        {
            readonly IPipeSpecification<ConsumeContext<TMessage>> _specification;

            public SagaMessageSplitFilterSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
            {
                _specification = specification;
            }

            public void Apply(IPipeBuilder<SagaConsumeContext<TSaga, TMessage>> builder)
            {
                _specification.Apply(new BuilderProxy(builder));
            }

            public IEnumerable<ValidationResult> Validate()
            {
                foreach (var validationResult in _specification.Validate())
                    yield return validationResult;
            }


            class BuilderProxy :
                IPipeBuilder<ConsumeContext<TMessage>>
            {
                readonly IPipeBuilder<SagaConsumeContext<TSaga, TMessage>> _builder;

                public BuilderProxy(IPipeBuilder<SagaConsumeContext<TSaga, TMessage>> builder)
                {
                    _builder = builder;
                }

                public void AddFilter(IFilter<ConsumeContext<TMessage>> filter)
                {
                    _builder.AddFilter(new SagaMessageSplitFilter<TSaga, TMessage>(filter));
                }
            }
        }
    }
}

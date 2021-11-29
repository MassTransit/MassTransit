namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Internals;
    using Middleware;


    public class ConsumerMessageSplitFilterSpecification<TConsumer, TMessage> :
        IPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>>
        where TMessage : class
        where TConsumer : class
    {
        readonly IPipeSpecification<ConsumeContext<TMessage>> _specification;

        public ConsumerMessageSplitFilterSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
        {
            _specification = specification;
        }

        public void Apply(IPipeBuilder<ConsumerConsumeContext<TConsumer, TMessage>> builder)
        {
            _specification.Apply(new BuilderProxy(builder));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (!typeof(TConsumer).HasInterface<IConsumer<TMessage>>())
                yield return this.Failure("MessageType", $"is not consumed by {TypeCache<TConsumer>.ShortName}");

            foreach (var validationResult in _specification.Validate())
                yield return validationResult;
        }


        class BuilderProxy :
            IPipeBuilder<ConsumeContext<TMessage>>
        {
            readonly IPipeBuilder<ConsumerConsumeContext<TConsumer, TMessage>> _builder;

            public BuilderProxy(IPipeBuilder<ConsumerConsumeContext<TConsumer, TMessage>> builder)
            {
                _builder = builder;
            }

            public void AddFilter(IFilter<ConsumeContext<TMessage>> filter)
            {
                _builder.AddFilter(new MessageSplitFilter<TConsumer, TMessage>(filter));
            }
        }
    }
}

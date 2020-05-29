namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using GreenPipes;
    using Pipeline.Filters;


    public class ConsumerPipeSpecification<TConsumer, TMessage> :
        IPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>>
        where TConsumer : class
        where TMessage : class
    {
        readonly IFilter<ConsumerConsumeContext<TConsumer, TMessage>> _filter;

        public ConsumerPipeSpecification(IFilter<ConsumerConsumeContext<TConsumer>> filter)
        {
            _filter = new ConsumerSplitFilter<TConsumer, TMessage>(filter);
        }

        public ConsumerPipeSpecification(IFilter<ConsumeContext<TMessage>> filter)
        {
            _filter = new MessageSplitFilter<TConsumer, TMessage>(filter);
        }

        public ConsumerPipeSpecification(IFilter<ConsumerConsumeContext<TConsumer, TMessage>> filter)
        {
            _filter = filter;
        }

        public void Apply(IPipeBuilder<ConsumerConsumeContext<TConsumer, TMessage>> builder)
        {
            builder.AddFilter(_filter);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_filter == null)
                yield return this.Failure("Filter", "must not be null");
        }
    }
}

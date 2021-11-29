namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


    public class ConsumerFilterSpecification<TConsumer, TMessage> :
        IPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>>
        where TConsumer : class
        where TMessage : class
    {
        readonly IFilter<ConsumerConsumeContext<TConsumer, TMessage>> _filter;

        public ConsumerFilterSpecification(IFilter<ConsumerConsumeContext<TConsumer>> filter)
        {
            _filter = new ConsumerSplitFilter<TConsumer, TMessage>(filter);
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

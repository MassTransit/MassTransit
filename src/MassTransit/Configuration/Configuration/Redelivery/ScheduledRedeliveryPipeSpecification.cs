namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


    public class ScheduledRedeliveryPipeSpecification<TMessage> :
        IPipeSpecification<ConsumeContext<TMessage>>,
        IRedeliveryPipeSpecification
        where TMessage : class
    {
        public void Apply(IPipeBuilder<ConsumeContext<TMessage>> builder)
        {
            builder.AddFilter(new ScheduleMessageRedeliveryFilter<TMessage>(Options));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public RedeliveryOptions Options { get; set; } = RedeliveryOptions.ReplaceMessageId;
    }
}

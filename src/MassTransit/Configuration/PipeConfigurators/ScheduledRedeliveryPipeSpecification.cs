namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using GreenPipes;
    using Pipeline.Filters;


    public class ScheduledRedeliveryPipeSpecification<TMessage> :
        IPipeSpecification<ConsumeContext<TMessage>>
        where TMessage : class
    {
        public void Apply(IPipeBuilder<ConsumeContext<TMessage>> builder)
        {
            builder.AddFilter(new ScheduleMessageRedeliveryFilter<TMessage>());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }
    }
}

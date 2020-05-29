namespace MassTransit.PipeConfigurators
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using Pipeline.Filters;


    public class MessageSchedulerPipeSpecification :
        IPipeSpecification<ConsumeContext>
    {
        readonly Uri _schedulerAddress;

        public MessageSchedulerPipeSpecification(Uri schedulerAddress)
        {
            _schedulerAddress = schedulerAddress;
        }

        public void Apply(IPipeBuilder<ConsumeContext> builder)
        {
            builder.AddFilter(new MessageSchedulerFilter(_schedulerAddress));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_schedulerAddress == null)
                yield return this.Failure("SchedulerAddress", "must not be null");
        }
    }
}

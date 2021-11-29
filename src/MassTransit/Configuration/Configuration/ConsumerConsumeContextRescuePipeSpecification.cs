namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;
    using Middleware.Rescue;


    public class ConsumerConsumeContextRescuePipeSpecification<T> :
        ExceptionSpecification,
        IPipeSpecification<ConsumerConsumeContext<T>>
        where T : class
    {
        readonly IPipe<ExceptionConsumerConsumeContext<T>> _rescuePipe;

        public ConsumerConsumeContextRescuePipeSpecification(IPipe<ExceptionConsumerConsumeContext<T>> rescuePipe)
        {
            _rescuePipe = rescuePipe;
        }

        public void Apply(IPipeBuilder<ConsumerConsumeContext<T>> builder)
        {
            builder.AddFilter(new RescueFilter<ConsumerConsumeContext<T>, ExceptionConsumerConsumeContext<T>>(_rescuePipe, Filter,
                (context, ex) => new RescueExceptionConsumerConsumeContext<T>(context, ex)));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_rescuePipe == null)
                yield return this.Failure("RescuePipe", "must not be null");
        }
    }
}

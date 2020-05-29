namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using Context;
    using GreenPipes;
    using GreenPipes.Filters;
    using GreenPipes.Specifications;


    public class ConsumeContextRescuePipeSpecification :
        ExceptionSpecification,
        IRescueConfigurator,
        IPipeSpecification<ConsumeContext>
    {
        readonly IPipe<ExceptionConsumeContext> _rescuePipe;

        public ConsumeContextRescuePipeSpecification(IPipe<ExceptionConsumeContext> rescuePipe)
        {
            _rescuePipe = rescuePipe;
        }

        public void Apply(IPipeBuilder<ConsumeContext> builder)
        {
            builder.AddFilter(new RescueFilter<ConsumeContext, ExceptionConsumeContext>(_rescuePipe, Filter,
                (context, ex) => new RescueExceptionConsumeContext(context, ex)));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_rescuePipe == null)
                yield return this.Failure("RescuePipe", "must not be null");
        }
    }


    public class ConsumeContextRescuePipeSpecification<T> :
        ExceptionSpecification,
        IRescueConfigurator,
        IPipeSpecification<ConsumeContext<T>>
        where T : class
    {
        readonly IPipe<ExceptionConsumeContext<T>> _rescuePipe;

        public ConsumeContextRescuePipeSpecification(IPipe<ExceptionConsumeContext<T>> rescuePipe)
        {
            _rescuePipe = rescuePipe;
        }

        public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            builder.AddFilter(new RescueFilter<ConsumeContext<T>, ExceptionConsumeContext<T>>(_rescuePipe, Filter,
                (context, ex) => new RescueExceptionConsumeContext<T>(context, ex)));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_rescuePipe == null)
                yield return this.Failure("RescuePipe", "must not be null");
        }
    }
}

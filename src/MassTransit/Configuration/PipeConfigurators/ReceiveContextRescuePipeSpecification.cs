namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using Context;
    using GreenPipes;
    using GreenPipes.Filters;
    using GreenPipes.Specifications;


    public class ReceiveContextRescuePipeSpecification :
        ExceptionSpecification,
        IRescueConfigurator,
        IPipeSpecification<ReceiveContext>
    {
        readonly IPipe<ExceptionReceiveContext> _rescuePipe;

        public ReceiveContextRescuePipeSpecification(IPipe<ExceptionReceiveContext> rescuePipe)
        {
            _rescuePipe = rescuePipe;
        }

        public void Apply(IPipeBuilder<ReceiveContext> builder)
        {
            builder.AddFilter(new RescueFilter<ReceiveContext, ExceptionReceiveContext>(_rescuePipe, Filter,
                (context, ex) => new RescueExceptionReceiveContext(context, ex)));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_rescuePipe == null)
                yield return this.Failure("RescuePipe", "must not be null");
        }
    }
}

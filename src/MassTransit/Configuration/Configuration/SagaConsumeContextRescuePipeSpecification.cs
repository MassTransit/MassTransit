namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;
    using Middleware.Rescue;


    public class SagaConsumeContextRescuePipeSpecification<T> :
        ExceptionSpecification,
        IPipeSpecification<SagaConsumeContext<T>>
        where T : class, ISaga
    {
        readonly IPipe<ExceptionSagaConsumeContext<T>> _rescuePipe;

        public SagaConsumeContextRescuePipeSpecification(IPipe<ExceptionSagaConsumeContext<T>> rescuePipe)
        {
            _rescuePipe = rescuePipe;
        }

        public void Apply(IPipeBuilder<SagaConsumeContext<T>> builder)
        {
            builder.AddFilter(new RescueFilter<SagaConsumeContext<T>, ExceptionSagaConsumeContext<T>>(_rescuePipe, Filter,
                (context, ex) => new RescueExceptionSagaConsumeContext<T>(context, ex)));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_rescuePipe == null)
                yield return this.Failure("RescuePipe", "must not be null");
        }
    }
}

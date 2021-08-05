namespace MassTransit.PipeConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Context;
    using GreenPipes;
    using Pipeline.Filters;


    public class TimeoutSpecification<T> :
        IPipeSpecification<ConsumeContext<T>>,
        ITimeoutConfigurator
        where T : class
    {
        public TimeSpan Timeout { get; set; }

        public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            builder.AddFilter(new TimeoutFilter<ConsumeContext<T>, TimeoutConsumeContext<T>>(Factory, Timeout));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        static TimeoutConsumeContext<T> Factory(ConsumeContext<T> context, CancellationToken cancellationToken)
        {
            return new TimeoutConsumeContext<T>(context, cancellationToken);
        }
    }
}

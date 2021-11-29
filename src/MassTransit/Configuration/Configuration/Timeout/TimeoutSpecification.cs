namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Middleware;
    using Middleware.Timeout;


    public class TimeoutSpecification<T> :
        IPipeSpecification<ConsumeContext<T>>,
        ITimeoutConfigurator
        where T : class
    {
        public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            builder.AddFilter(new TimeoutFilter<ConsumeContext<T>, TimeoutConsumeContext<T>>(Factory, Timeout));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (Timeout <= TimeSpan.Zero)
                yield return this.Failure("Timeout", "must be > TimeSpan.Zero");
        }

        public TimeSpan Timeout { get; set; }

        static TimeoutConsumeContext<T> Factory(ConsumeContext<T> context, CancellationToken cancellationToken)
        {
            return new TimeoutConsumeContext<T>(context, cancellationToken);
        }
    }
}

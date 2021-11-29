namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Middleware;
    using Middleware.Timeout;


    public class CompensateContextTimeoutSpecification<TArguments> :
        IPipeSpecification<CompensateContext<TArguments>>,
        ITimeoutConfigurator
        where TArguments : class
    {
        public void Apply(IPipeBuilder<CompensateContext<TArguments>> builder)
        {
            builder.AddFilter(new TimeoutFilter<CompensateContext<TArguments>, TimeoutCompensateContext<TArguments>>(Factory, Timeout));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (Timeout <= TimeSpan.Zero)
                yield return this.Failure("Timeout", "must be > TimeSpan.Zero");
        }

        public TimeSpan Timeout { get; set; }

        static TimeoutCompensateContext<TArguments> Factory(CompensateContext<TArguments> context, CancellationToken cancellationToken)
        {
            return new TimeoutCompensateContext<TArguments>(context, cancellationToken);
        }
    }
}

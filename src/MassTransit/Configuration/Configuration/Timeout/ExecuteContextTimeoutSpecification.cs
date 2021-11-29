namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Middleware;
    using Middleware.Timeout;


    public class ExecuteContextTimeoutSpecification<TArguments> :
        IPipeSpecification<ExecuteContext<TArguments>>,
        ITimeoutConfigurator
        where TArguments : class
    {
        public void Apply(IPipeBuilder<ExecuteContext<TArguments>> builder)
        {
            builder.AddFilter(new TimeoutFilter<ExecuteContext<TArguments>, TimeoutExecuteContext<TArguments>>(Factory, Timeout));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (Timeout <= TimeSpan.Zero)
                yield return this.Failure("Timeout", "must be > TimeSpan.Zero");
        }

        public TimeSpan Timeout { get; set; }

        static TimeoutExecuteContext<TArguments> Factory(ExecuteContext<TArguments> context, CancellationToken cancellationToken)
        {
            return new TimeoutExecuteContext<TArguments>(context, cancellationToken);
        }
    }
}

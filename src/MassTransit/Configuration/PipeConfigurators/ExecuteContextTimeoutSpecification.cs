namespace MassTransit.PipeConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Courier;
    using Courier.Contexts;
    using GreenPipes;
    using Pipeline.Filters;


    public class ExecuteContextTimeoutSpecification<TArguments> :
        IPipeSpecification<ExecuteContext<TArguments>>,
        ITimeoutConfigurator
        where TArguments : class
    {
        public TimeSpan Timeout { get; set; }

        public void Apply(IPipeBuilder<ExecuteContext<TArguments>> builder)
        {
            builder.AddFilter(
                new TimeoutFilter<ExecuteContext<TArguments>, TimeoutExecuteContext<TArguments>>(Factory, Timeout));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        static TimeoutExecuteContext<TArguments> Factory(ExecuteContext<TArguments> context, CancellationToken cancellationToken)
        {
            return new TimeoutExecuteContext<TArguments>(context, cancellationToken);
        }
    }
}

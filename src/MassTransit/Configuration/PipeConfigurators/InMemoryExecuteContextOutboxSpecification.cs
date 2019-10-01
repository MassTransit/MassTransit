namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using Courier;
    using Courier.Contexts;
    using GreenPipes;
    using Pipeline.Filters;


    public class InMemoryExecuteContextOutboxSpecification<TArguments> :
        IPipeSpecification<ExecuteContext<TArguments>>
        where TArguments : class
    {
        public void Apply(IPipeBuilder<ExecuteContext<TArguments>> builder)
        {
            builder.AddFilter(new InMemoryOutboxFilter<ExecuteContext<TArguments>, InMemoryOutboxExecuteContext<TArguments>>(Factory));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        static InMemoryOutboxExecuteContext<TArguments> Factory(ExecuteContext<TArguments> context)
        {
            return new InMemoryOutboxExecuteContext<TArguments>(context);
        }
    }
}

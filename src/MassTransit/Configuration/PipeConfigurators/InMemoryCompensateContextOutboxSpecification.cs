namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using Courier;
    using Courier.Contexts;
    using GreenPipes;
    using Pipeline.Filters;


    public class InMemoryCompensateContextOutboxSpecification<TArguments> :
        IPipeSpecification<CompensateContext<TArguments>>
        where TArguments : class
    {
        public void Apply(IPipeBuilder<CompensateContext<TArguments>> builder)
        {
            builder.AddFilter(new InMemoryOutboxFilter<CompensateContext<TArguments>, InMemoryOutboxCompensateContext<TArguments>>(Factory));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        static InMemoryOutboxCompensateContext<TArguments> Factory(CompensateContext<TArguments> context)
        {
            return new InMemoryOutboxCompensateContext<TArguments>(context);
        }
    }
}

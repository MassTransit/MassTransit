namespace MassTransit.Monitoring.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Configuration;
    using Middleware;


    public class InstrumentReceiveSpecification :
        IPipeSpecification<ReceiveContext>
    {
        public void Apply(IPipeBuilder<ReceiveContext> builder)
        {
            builder.AddFilter(new InstrumentReceiveFilter());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}

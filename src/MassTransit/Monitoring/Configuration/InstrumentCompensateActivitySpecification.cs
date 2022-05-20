namespace MassTransit.Monitoring.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Configuration;
    using Middleware;


    public class InstrumentCompensateActivitySpecification<TActivity, TLog> :
        IPipeSpecification<CompensateActivityContext<TActivity, TLog>>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        public void Apply(IPipeBuilder<CompensateActivityContext<TActivity, TLog>> builder)
        {
            builder.AddFilter(new InstrumentCompensateActivityFilter<TActivity, TLog>());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}

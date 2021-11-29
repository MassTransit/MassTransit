namespace MassTransit.PrometheusIntegration.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using Courier;
    using MassTransit.Configuration;
    using Middleware;


    public class PrometheusExecuteActivitySpecification<TActivity, TArguments> :
        IPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        public void Apply(IPipeBuilder<ExecuteActivityContext<TActivity, TArguments>> builder)
        {
            builder.AddFilter(new PrometheusExecuteActivityFilter<TActivity, TArguments>());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}

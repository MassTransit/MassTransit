namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


    /// <summary>
    /// Configures the Latest filter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LatestPipeSpecification<T> :
        IPipeSpecification<T>,
        ILatestConfigurator<T>
        where T : class, PipeContext
    {
        LatestFilterCreated<T> _created;

        LatestFilterCreated<T> ILatestConfigurator<T>.Created
        {
            set => _created = value;
        }

        void IPipeSpecification<T>.Apply(IPipeBuilder<T> builder)
        {
            var filter = new LatestFilter<T>();
            builder.AddFilter(filter);

            _created?.Invoke(filter);
        }

        IEnumerable<ValidationResult> ISpecification.Validate()
        {
            yield break;
        }
    }
}

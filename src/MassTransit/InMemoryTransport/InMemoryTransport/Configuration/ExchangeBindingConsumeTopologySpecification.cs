namespace MassTransit.InMemoryTransport.Configuration
{
    using System.Collections.Generic;


    /// <summary>
    /// Used to bind an exchange to the consuming queue's exchange
    /// </summary>
    public class ExchangeBindingConsumeTopologySpecification :
        IInMemoryConsumeTopologySpecification
    {
        readonly string _exchange;

        public ExchangeBindingConsumeTopologySpecification(string exchange)
        {
            _exchange = exchange;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Apply(IInMemoryConsumeTopologyBuilder builder)
        {
            builder.ExchangeBind(_exchange, builder.Exchange);
        }
    }
}

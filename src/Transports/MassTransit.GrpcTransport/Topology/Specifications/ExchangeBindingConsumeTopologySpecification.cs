namespace MassTransit.GrpcTransport.Topology.Specifications
{
    using System.Collections.Generic;
    using GreenPipes;
    using GrpcTransport.Builders;


    /// <summary>
    /// Used to bind an exchange to the consuming queue's exchange
    /// </summary>
    public class ExchangeBindingConsumeTopologySpecification :
        IGrpcConsumeTopologySpecification
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

        public void Apply(IGrpcConsumeTopologyBuilder builder)
        {
            builder.ExchangeBind(_exchange, builder.Exchange);
        }
    }
}
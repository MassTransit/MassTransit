namespace MassTransit.RabbitMqTransport.Configuration
{
    using System.Collections.Generic;
    using Topology;


    /// <summary>
    /// Used to bind an exchange to the consuming queue's exchange
    /// </summary>
    public class ExchangeToExchangeBindingConsumeTopologySpecification :
        IRabbitMqConsumeTopologySpecification
    {
        readonly ExchangeToExchangeBinding _binding;

        public ExchangeToExchangeBindingConsumeTopologySpecification(ExchangeToExchangeBinding binding)
        {
            _binding = binding;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            var source = _binding.Source;

            var sourceHandle = builder.ExchangeDeclare(source.ExchangeName, source.ExchangeType, source.Durable, source.AutoDelete, source.ExchangeArguments);

            var destination = _binding.Destination;

            var destinationHandle = builder.ExchangeDeclare(destination.ExchangeName, destination.ExchangeType, destination.Durable, destination.AutoDelete,
                destination.ExchangeArguments);

            var bindingHandle = builder.ExchangeBind(sourceHandle, destinationHandle, _binding.RoutingKey, _binding.Arguments);
        }
    }
}

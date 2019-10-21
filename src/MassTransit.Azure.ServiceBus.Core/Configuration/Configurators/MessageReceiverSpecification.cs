namespace MassTransit.Azure.ServiceBus.Core.Configurators
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using EndpointSpecifications;
    using GreenPipes;
    using MassTransit.Configuration;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Filters;
    using MassTransit.Pipeline.Pipes;
    using Transports;


    public abstract class MessageReceiverSpecification :
        ReceiveSpecification,
        IReceiverConfigurator,
        ISpecification
    {
        readonly IReceiveEndpointConfiguration _configuration;

        protected MessageReceiverSpecification(IReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
            InputAddress = new Uri("sb://localhost/");
        }

        public Uri InputAddress { get; set; }

        public void AddDependency(IReceiveEndpointObserverConnector connector)
        {
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;
        }

        protected IReceivePipe CreateReceivePipe()
        {
            var builder = new MessageReceiverBuilder(_configuration);

            foreach (var specification in Specifications)
                specification.Configure(builder);

            ReceivePipeConfigurator.UseFilter(new DeserializeFilter(_configuration.Serialization.Deserializer, _configuration.ConsumePipe));

            IPipe<ReceiveContext> receivePipe = ReceivePipeConfigurator.Build();

            return new ReceivePipe(receivePipe, _configuration.ConsumePipe);
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _configuration.ConnectReceiveEndpointObserver(observer);
        }
    }
}

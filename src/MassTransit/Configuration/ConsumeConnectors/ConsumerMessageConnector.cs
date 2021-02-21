namespace MassTransit.ConsumeConnectors
{
    using System;
    using System.Reflection;
    using ConsumerSpecifications;
    using GreenPipes;
    using Pipeline;
    using Pipeline.Filters;
    using Topology;


    public class ConsumerMessageConnector<TConsumer, TMessage> :
        IConsumerMessageConnector<TConsumer>
        where TConsumer : class
        where TMessage : class
    {
        const ConnectPipeOptions NotConfigureConsumeTopology = ConnectPipeOptions.All & ~ConnectPipeOptions.ConfigureConsumeTopology;
        readonly IFilter<ConsumerConsumeContext<TConsumer, TMessage>> _consumeFilter;

        public ConsumerMessageConnector(IFilter<ConsumerConsumeContext<TConsumer, TMessage>> consumeFilter)
        {
            _consumeFilter = consumeFilter;

            var attribute = typeof(TMessage).GetCustomAttribute<ConfigureConsumeTopologyAttribute>();
            if (attribute != null)
                ConfigureConsumeTopology = attribute.ConfigureConsumeTopology;
        }

        bool ConfigureConsumeTopology { get; } = true;

        public Type MessageType => typeof(TMessage);

        public IConsumerMessageSpecification<TConsumer> CreateConsumerMessageSpecification()
        {
            return new ConsumerMessageSpecification<TConsumer, TMessage>();
        }

        public ConnectHandle ConnectConsumer(IConsumePipeConnector consumePipe, IConsumerFactory<TConsumer> consumerFactory,
            IConsumerSpecification<TConsumer> specification)
        {
            IConsumerMessageSpecification<TConsumer, TMessage> messageSpecification = specification.GetMessageSpecification<TMessage>();

            IPipe<ConsumerConsumeContext<TConsumer, TMessage>> consumerPipe = messageSpecification.Build(_consumeFilter);

            IPipe<ConsumeContext<TMessage>> messagePipe = messageSpecification.BuildMessagePipe(x =>
            {
                x.UseFilter(new ConsumerMessageFilter<TConsumer, TMessage>(consumerFactory, consumerPipe));
            });

            return ConfigureConsumeTopology
                ? consumePipe.ConnectConsumePipe(messagePipe)
                : consumePipe.ConnectConsumePipe(messagePipe, NotConfigureConsumeTopology);
        }
    }
}

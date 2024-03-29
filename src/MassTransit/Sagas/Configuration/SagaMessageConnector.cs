namespace MassTransit.Configuration
{
    using System;


    public partial class SagaConnector<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        public abstract class SagaMessageConnector :
            ISagaMessageConnector<TSaga>
        {
            const ConnectPipeOptions NotConfigureConsumeTopology = ConnectPipeOptions.All & ~ConnectPipeOptions.ConfigureConsumeTopology;
            readonly IFilter<SagaConsumeContext<TSaga, TMessage>> _consumeFilter;

            protected SagaMessageConnector(IFilter<SagaConsumeContext<TSaga, TMessage>> consumeFilter)
            {
                _consumeFilter = consumeFilter;
            }

            protected virtual bool ConfigureConsumeTopology { get; } = true;

            public Type MessageType => typeof(TMessage);

            public ISagaMessageSpecification<TSaga> CreateSagaMessageSpecification()
            {
                return new SagaMessageSpecification();
            }

            public ConnectHandle ConnectSaga(IConsumePipeConnector consumePipe, ISagaRepository<TSaga> repository, ISagaSpecification<TSaga> specification)
            {
                ISagaMessageSpecification<TSaga, TMessage> messageSpecification = specification.GetMessageSpecification<TMessage>();

                IPipe<SagaConsumeContext<TSaga, TMessage>> consumerPipe = messageSpecification.BuildConsumerPipe(_consumeFilter);

                IPipe<ConsumeContext<TMessage>> messagePipe = messageSpecification.BuildMessagePipe(x =>
                {
                    specification.ConfigureMessagePipe(x);

                    ConfigureMessagePipe(x, repository, consumerPipe);
                });

                return ConfigureConsumeTopology
                    ? consumePipe.ConnectConsumePipe(messagePipe)
                    : consumePipe.ConnectConsumePipe(messagePipe, NotConfigureConsumeTopology);
            }

            /// <summary>
            /// Configure the message pipe that is prior to the saga repository
            /// </summary>
            /// <param name="configurator">The pipe configurator</param>
            /// <param name="repository"></param>
            /// <param name="sagaPipe"></param>
            protected abstract void ConfigureMessagePipe(IPipeConfigurator<ConsumeContext<TMessage>> configurator, ISagaRepository<TSaga> repository,
                IPipe<SagaConsumeContext<TSaga, TMessage>> sagaPipe);
        }
    }
}

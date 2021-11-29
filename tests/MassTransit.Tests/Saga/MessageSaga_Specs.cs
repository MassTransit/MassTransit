namespace MassTransit.Tests.Saga
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Configuring_a_message_in_a_saga :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_all_the_stuff()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            await _message.Task;

            await _consumerOnly.Task;

            await _consumerMessage.Task;
        }

        TaskCompletionSource<PingMessage> _message;
        TaskCompletionSource<Tuple<MySaga, PingMessage>> _consumerMessage;
        TaskCompletionSource<MySaga> _consumerOnly;
        InMemorySagaRepository<MySaga> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _message = GetTask<PingMessage>();
            _consumerOnly = GetTask<MySaga>();
            _consumerMessage = GetTask<Tuple<MySaga, PingMessage>>();

            _repository = new InMemorySagaRepository<MySaga>();

            configurator.Saga(_repository, cfg =>
            {
                cfg.UseExecute(context => _consumerOnly.TrySetResult(context.Saga));
                cfg.Message<PingMessage>(m => m.UseExecute(context => _message.TrySetResult(context.Message)));
                cfg.SagaMessage<PingMessage>(m => m.UseExecute(context => _consumerMessage.TrySetResult(Tuple.Create(context.Saga, context.Message))));
            });
        }


        class MySaga :
            InitiatedBy<PingMessage>,
            ISaga
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return Task.CompletedTask;
            }

            public Guid CorrelationId { get; set; }
        }
    }
}

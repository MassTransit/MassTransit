namespace MassTransit.Tests.MessageData
{
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.MessageData;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Sending_an_array_of_objects_with_message_data :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_properly_initialize()
        {
            var firstBody = new byte[10000];
            var secondBody = new byte[10000];

            Task<ConsumeContext<ReceiveFault>> receiveFault = ConnectPublishHandler<ReceiveFault>();
            Task<ConsumeContext<Fault<Documents>>> fault = ConnectPublishHandler<Fault<Documents>>();

            await InputQueueSendEndpoint.Send<Documents>(new {Bodies = new[] {new {Body = firstBody}, new {Body = secondBody}}});

            await Task.WhenAny(receiveFault, fault, _handled.Task);

            if (_handled.Task.IsCompleted)
            {
            }
            else if (fault.IsCompleted)
            {
                ConsumeContext<Fault<Documents>> faulted = await fault;

                Assert.Fail(faulted.Message.Exceptions.FirstOrDefault()?.Message);
            }
            else
            {
                ConsumeContext<ReceiveFault> faulted = await receiveFault;

                Assert.Fail(faulted.Message.Exceptions.FirstOrDefault()?.Message);
            }
        }

        TaskCompletionSource<ConsumeContext<Documents>> _handled;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseMessageData(new InMemoryMessageDataRepository());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = GetTask<ConsumeContext<Documents>>();
            configurator.Handler<Documents>(async context =>
            {
                Assert.That(context.Message.Bodies, Is.Not.Null);
                Assert.That(context.Message.Bodies.Length, Is.EqualTo(2));

                Assert.That(context.Message.Bodies[0].Body, Is.Not.Null);
                Assert.That(context.Message.Bodies[0].Body.HasValue);

                byte[] bodyValue = await context.Message.Bodies[0].Body.Value;
                Assert.That(bodyValue, Is.Not.Null);
                Assert.That(bodyValue.Length, Is.EqualTo(10000));


                _handled.SetResult(context);
            });
        }


        public interface Documents
        {
            Document[] Bodies { get; }
        }


        public interface Document
        {
            MessageData<byte[]> Body { get; }
        }
    }
}

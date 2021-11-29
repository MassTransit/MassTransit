namespace MassTransit.Tests.Serialization
{
    namespace Polymorphic
    {
        using System.Collections.Generic;
        using System.Threading.Tasks;
        using Newtonsoft.Json;
        using NUnit.Framework;
        using TestFramework;


        // message interface
        public interface ITestMessage
        {
            [JsonProperty(TypeNameHandling = TypeNameHandling.Objects)]
            TestBaseClass Data { get; }
        }


        // message implementation
        public class TestMessage : ITestMessage
        {
            public TestBaseClass Data { get; set; }
        }


        public interface ITestArrayMessage
        {
            [JsonProperty(ItemTypeNameHandling = TypeNameHandling.Auto)]
            TestBaseClass[] Data { get; }
        }


        public class TestArrayMessage :
            ITestArrayMessage
        {
            public TestBaseClass[] Data { get; set; }
        }


        public interface ITestListMessage
        {
            [JsonProperty(ItemTypeNameHandling = TypeNameHandling.Auto)]
            IList<TestBaseClass> Data { get; }
        }


        public class TestListMessage :
            ITestListMessage
        {
            public IList<TestBaseClass> Data { get; set; }
        }


        public abstract class TestBaseClass
        {
        }


        public class TestConcreteClass : TestBaseClass
        {
        }


        [TestFixture]
        public class PolymorphicProperty_Specs :
            InMemoryTestFixture
        {
            [Test]
            public async Task Verify_consumed_message_contains_property()
            {
                ITestMessage message = new TestMessage { Data = new TestConcreteClass() };

                await InputQueueSendEndpoint.Send(message);

                ConsumeContext<ITestMessage> context = await _handled;

                Assert.IsInstanceOf<TestConcreteClass>(context.Message.Data);
            }

            Task<ConsumeContext<ITestMessage>> _handled;

            protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
            {
                configurator.UseNewtonsoftJsonSerializer();
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                _handled = Handled<ITestMessage>(configurator);
            }
        }


        [TestFixture]
        public class PolymorphicProperty_Array_Specs :
            InMemoryTestFixture
        {
            [Test]
            public async Task Verify_consumed_message_contains_property()
            {
                ITestArrayMessage message = new TestArrayMessage { Data = new TestBaseClass[] { new TestConcreteClass() } };

                await InputQueueSendEndpoint.Send(message);

                await Task.WhenAny(_handled, _faulted);
                if (_faulted.IsCompleted)
                    Assert.Fail("Should not faulted");

                ConsumeContext<ITestArrayMessage> context = await _handled;

                Assert.That(context.Message.Data, Is.Not.Null);
                Assert.That(context.Message.Data.Length, Is.EqualTo(1));

                Assert.IsInstanceOf<TestConcreteClass>(context.Message.Data[0]);
            }

            Task<ConsumeContext<ITestArrayMessage>> _handled;
            Task<ConsumeContext<ReceiveFault>> _faulted;

            protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
            {
                configurator.UseNewtonsoftJsonSerializer();
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                _handled = Handled<ITestArrayMessage>(configurator);
                _faulted = Handled<ReceiveFault>(configurator);
            }
        }


        [TestFixture]
        public class PolymorphicProperty_List_Specs :
            InMemoryTestFixture
        {
            [Test]
            public async Task Verify_consumed_message_contains_property()
            {
                ITestListMessage message = new TestListMessage { Data = new List<TestBaseClass> { new TestConcreteClass() } };

                await InputQueueSendEndpoint.Send(message);

                await Task.WhenAny(_handled, _faulted);
                if (_faulted.IsCompleted)
                    Assert.Fail("Should not faulted");

                ConsumeContext<ITestListMessage> context = await _handled;

                Assert.That(context.Message.Data, Is.Not.Null);
                Assert.That(context.Message.Data.Count, Is.EqualTo(1));

                Assert.IsInstanceOf<TestConcreteClass>(context.Message.Data[0]);
            }

            Task<ConsumeContext<ITestListMessage>> _handled;
            Task<ConsumeContext<ReceiveFault>> _faulted;

            protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
            {
                configurator.UseNewtonsoftJsonSerializer();
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                _handled = Handled<ITestListMessage>(configurator);
                _faulted = Handled<ReceiveFault>(configurator);
            }
        }
    }
}

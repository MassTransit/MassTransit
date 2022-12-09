namespace MassTransit.Tests.Testing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Shouldly;


    public class When_a_saga_is_being_tested2
    {
        InMemoryTestHarness _harness;
        Guid _sagaId;
        string _testValueA;

        [OneTimeSetUp]
        public async Task A_saga_is_being_tested()
        {
            _sagaId = Guid.NewGuid();
            _testValueA = "TestValueA";

            _harness = new InMemoryTestHarness();
            _harness.OnConfigureInMemoryBus += OnConfigureInMemoryBus;

            _ = _harness.Saga<TestSaga>();

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A
            {
                CorrelationId = _sagaId,
                Value = _testValueA,
                Xs = new List<X> { new X { Foo = "Foo" }, new Y { Bar = "Bar" } }
            });
        }

        private void OnConfigureInMemoryBus(IInMemoryBusFactoryConfigurator x)
        {
            x.UseNewtonsoftJsonSerializer();
            x.ConfigureNewtonsoftJsonSerializer(ConfigureSerializerSettings);

            x.UseNewtonsoftJsonDeserializer();
            // Remove this: X instead of Y is returned
            x.ConfigureNewtonsoftJsonDeserializer(ConfigureSerializerSettings);
        }

        private JsonSerializerSettings ConfigureSerializerSettings(JsonSerializerSettings s)
        {
            s.TypeNameHandling = TypeNameHandling.Auto;
            s.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full;
            return s;
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }

        [Test]
        public void Serialization_roundtrip_should_work()
        {
            JsonSerializer serializer = JsonSerializer.Create(ConfigureSerializerSettings(new JsonSerializerSettings()));
            A a = new A { Xs = new List<X> { new X { Foo = "Foo" }, new Y { Bar = "Bar" } } };

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            using (JsonWriter writer = new JsonTextWriter(sw))
                serializer.Serialize(writer, a);

            using (StringReader sw = new StringReader(sb.ToString()))
            using (JsonReader reader = new JsonTextReader(sw))
            {
                A result = serializer.Deserialize<A>(reader);
                result.ShouldBeEquivalentTo(a);
            }
        }

        [Test]
        public void Should_receive_the_message_type_a_with_inheritance()
        {
            A message = (A)_harness.Consumed.Select<A>().Single().MessageObject;

            message.Xs[0].ShouldBeOfType(typeof(X));
            message.Xs[1].ShouldBeOfType(typeof(Y));
        }

        class TestSaga :
            ISaga,
            InitiatedBy<A>,
            Orchestrates<B>,
            Observes<C, TestSaga>
        {
            protected TestSaga()
            {
            }

            public TestSaga(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public string ValueA { get; private set; }

            public async Task Consume(ConsumeContext<A> context)
            {
                ValueA = context.Message.Value;
                await context.Publish(new Aa { CorrelationId = CorrelationId });
            }

            public Guid CorrelationId { get; set; }

            public async Task Consume(ConsumeContext<C> message)
            {
            }

            public Expression<Func<TestSaga, C, bool>> CorrelationExpression
            {
                get { return (saga, message) => saga.CorrelationId.ToString() == message.CorrelationId; }
            }

            public async Task Consume(ConsumeContext<B> message)
            {
            }
        }


        class A :
            CorrelatedBy<Guid>
        {
            public string Value { get; set; }
            public Guid CorrelationId { get; set; }
            public List<X> Xs { get; set; }
        }

        class Aa :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }

        class X
        {
            public string Foo { get; set; }

        }

        class Y : X
        {
            public string Bar { get; set; }
        }


        class B :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class C
        {
            public string CorrelationId { get; set; }
        }
    }
}

namespace MassTransit.Tests.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using TestFramework;


    public class When_a_saga_is_being_tested
    {
        InMemoryTestHarness _harness;
        SagaTestHarness<TestSaga> _saga;
        Guid _sagaId;
        string _testValueA;

        [OneTimeSetUp]
        public async Task A_saga_is_being_tested()
        {
            _sagaId = Guid.NewGuid();
            _testValueA = "TestValueA";

            _harness = new InMemoryTestHarness();
            _harness.TestTimeout = TimeSpan.FromMinutes(10);

            _harness.OnConfigureInMemoryBus += e =>
            {
                BusTestFixture.ConfigureBusDiagnostics(e);

                e.UseNewtonsoftJsonSerializer();
                e.ConfigureNewtonsoftJsonDeserializer(x =>
                {
                    x.TypeNameHandling = TypeNameHandling.Auto;
                    x.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full;
                    return x;
                });
                e.ConfigureNewtonsoftJsonSerializer(x =>
                {
                    x.TypeNameHandling = TypeNameHandling.Auto;
                    x.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full;
                    return x;
                });
            };
            _saga = _harness.Saga<TestSaga>();

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A
            {
                CorrelationId = _sagaId,
                Value = _testValueA,
                Xs = new List<X>
                {
                    new X { Foo = "Foo" },
                    new Y { Bar = "Bar" }
                }
            });
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
            _harness.Dispose();
        }

        [Test]
        public void Should_send_the_initial_message_to_the_consumer()
        {
            Assert.That(_harness.Sent.Select<A>().Any(), Is.True);
        }

        [Test]
        public void Should_receive_the_message_type_a()
        {
            Assert.That(_harness.Consumed.Select<A>().Any(), Is.True);
        }

        [Test]
        public void Should_create_a_new_saga_for_the_message()
        {
            Assert.That(_saga.Created.Select(x => x.CorrelationId == _sagaId).Any(), Is.True);
        }

        [Test]
        public void Should_have_the_saga_instance_with_the_value()
        {
            var saga = _saga.Created.Contains(_sagaId);
            Assert.That(saga, Is.Not.Null);

            Assert.That(saga.ValueA, Is.EqualTo(_testValueA));
        }

        [Test]
        public void Should_have_published_event_message()
        {
            Assert.That(_harness.Published.Select<Aa>().Any(), Is.True);
        }

        [Test]
        public void Should_have_called_the_consumer_method()
        {
            Assert.That(_saga.Consumed.Select<A>().Any(), Is.True);
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


        class A : CorrelatedBy<Guid>
        {
            public string Value { get; set; }
            public Guid CorrelationId { get; set; }
            public List<X> Xs { get; set; } // NEW List with X's
        }


        class X // NEW
        {
            public string Foo { get; set; }
        }


        class Y : X // NEW
        {
            public string Bar { get; set; }
        }


        class Aa :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
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

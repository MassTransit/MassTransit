namespace MassTransit.Tests.Testing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Events;
    using MassTransit.Metadata;
    using MassTransit.Testing;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    public class When_a_state_machine_saga_is_being_tested
    {
        InMemoryTestHarness _harness;
        ISagaStateMachineTestHarness<TestStateMachineSaga, TestInstance> _saga;
        TestStateMachineSaga _testStateMachineSaga;

        Guid _sagaId;
        Guid _faultId = Guid.NewGuid();
        Guid _faultedMessageId = Guid.NewGuid();
        DateTime _timestamp = DateTime.Now;
        string[] _faultMessageTypes = new[] { "FaultMessageType" };

        [OneTimeSetUp]
        public async Task A_state_machine_saga_is_being_tested()
        {
            _sagaId = Guid.NewGuid();
            _testStateMachineSaga = new TestStateMachineSaga();

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
            _saga = _harness.StateMachineSaga<TestInstance, TestStateMachineSaga>(_testStateMachineSaga);

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new StartMessage
            {
                CorrelationId = _sagaId,
                TestKey = "abc"
            });
            await _harness.InputQueueSendEndpoint.Send<Fault<ExecuteRequest>>(new
            {
                FaultId = _faultId,
                FaultedMessageId = _faultedMessageId,
                Exceptions = new FaultExceptionInfo[] { new FaultExceptionInfo() },
                Timestamp = _timestamp,
                Message = new ExecuteRequest(),
                FaultMessageTypes = _faultMessageTypes
            });
        }

        [Test]
        public async Task Should_receive_the_fault_with_data()
        {
            await _saga.Exists(_sagaId);
            _harness.Consumed.Select<StartMessage>().Any().ShouldBeTrue();
            _harness.Consumed.Select<Fault<ExecuteRequest>>().Any().ShouldBeTrue();

            Fault<ExecuteRequest> result = ((Fault<ExecuteRequest>)_harness.Consumed.Select<Fault<ExecuteRequest>>().Single().MessageObject);
            result.FaultId.ShouldBe(_faultId);
            result.FaultedMessageId.ShouldBe(_faultedMessageId);
            result.Timestamp.ShouldBe(_timestamp);
            result.FaultMessageTypes.ShouldBeEquivalentTo(_faultMessageTypes);
            result.Message.ShouldNotBeNull();
            result.Exceptions.ShouldNotBeNull();
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }

        public class TestInstance :
            SagaStateMachineInstance
        {
            public string CurrentState { get; set; }
            public string Key { get; set; }

            public Guid? FooRequestId { get; set; }

            public Guid CorrelationId { get; set; }
        }

        public class TestStateMachineSaga :
          MassTransitStateMachine<TestInstance>
        {
            public TestStateMachineSaga()
            {
                InstanceState(x => x.CurrentState);

                Event(() => Start, x =>
                {
                    x.CorrelateById(x => x.CorrelationId.Value);
                    x.SelectId(context => context.CorrelationId.Value);
                    x.InsertOnInitial = true;
                });

                Request(() => DoSomething, x => x.FooRequestId, r =>
                {
                    r.Timeout = TimeSpan.Zero;
                });

                Initially(When(Start)
                    .TransitionTo(Active));

                During(Active, When(DoSomething.Faulted)
                    .TransitionTo(Final));
            }

            public State Active { get; private set; }
            public State Done { get; private set; }

            public Event<StartMessage> Start { get; private set; }
            public Request<TestInstance, ExecuteRequest, ExecuteResponse> DoSomething { get; private set; }
        }

        public class StartMessage :
            CorrelatedBy<Guid>
        {
            public string TestKey { get; set; }
            public Guid CorrelationId { get; set; }
        }

        public class ExecuteRequest
        { }

        public class ExecuteResponse
        { }
    }
}

namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    namespace Telephone_Sample
    {
        using System;
        using System.Diagnostics;
        using System.Threading.Tasks;
        using NUnit.Framework;
        using SagaStateMachine;
        using Visualizer;


        [TestFixture]
        public class A_simple_phone_call
        {
            [Test]
            public async Task Should_be_short_and_sweet()
            {
                var phone = new PrincessModelTelephone();
                await _machine.RaiseEvent(phone, _machine.ServiceEstablished, new PhoneServiceEstablished { Digits = "555-1212" });

                await _machine.RaiseEvent(phone, x => x.CallDialed);
                await _machine.RaiseEvent(phone, x => x.CallConnected);

                await Task.Delay(50);

                await _machine.RaiseEvent(phone, x => x.HungUp);

                Assert.AreEqual(_machine.OffHook.Name, phone.CurrentState);
                Assert.GreaterOrEqual(phone.CallTimer.ElapsedMilliseconds, 45);
            }

            PhoneStateMachine _machine;

            [OneTimeSetUp]
            public void Setup()
            {
                _machine = new PhoneStateMachine();
            }
        }


        [TestFixture]
        public class Visualize
        {
            [Test]
            public void Draw()
            {
                var machine = new PhoneStateMachine();
                var generator = new StateMachineGraphvizGenerator(machine.GetGraph());

                var dotFile = generator.CreateDotFile();

                Console.WriteLine(dotFile);
            }
        }


        [TestFixture]
        public class A_short_time_on_hold
        {
            [Test]
            public async Task Should_be_short_and_sweet()
            {
                var phone = new PrincessModelTelephone();
                await _machine.RaiseEvent(phone, _machine.ServiceEstablished, new PhoneServiceEstablished { Digits = "555-1212" });

                await _machine.RaiseEvent(phone, x => x.CallDialed);
                await _machine.RaiseEvent(phone, x => x.CallConnected);

                await Task.Delay(50);

                await _machine.RaiseEvent(phone, x => x.PlacedOnHold);
                await _machine.RaiseEvent(phone, x => x.TakenOffHold);
                await _machine.RaiseEvent(phone, x => x.HungUp);

                Assert.AreEqual(_machine.OffHook.Name, phone.CurrentState);
                Assert.GreaterOrEqual(phone.CallTimer.ElapsedMilliseconds, 45);
            }

            PhoneStateMachine _machine;

            [OneTimeSetUp]
            public void Setup()
            {
                _machine = new PhoneStateMachine();
            }
        }


        [TestFixture]
        public class An_extended_time_on_hold
        {
            [Test]
            public async Task Should_end__badly()
            {
                var phone = new PrincessModelTelephone();
                await _machine.RaiseEvent(phone, _machine.ServiceEstablished, new PhoneServiceEstablished { Digits = "555-1212" });

                await _machine.RaiseEvent(phone, x => x.CallDialed);
                await _machine.RaiseEvent(phone, x => x.CallConnected);
                await _machine.RaiseEvent(phone, x => x.PlacedOnHold);

                await Task.Delay(50);

                await _machine.RaiseEvent(phone, x => x.HungUp);

                Assert.AreEqual(_machine.OffHook.Name, phone.CurrentState);
                Assert.GreaterOrEqual(phone.CallTimer.ElapsedMilliseconds, 45);
            }

            PhoneStateMachine _machine;

            [OneTimeSetUp]
            public void Setup()
            {
                _machine = new PhoneStateMachine();
            }
        }


        class PrincessModelTelephone :
            SagaStateMachineInstance
        {
            public PrincessModelTelephone()
            {
                CallTimer = new Stopwatch();
            }

            public string CurrentState { get; set; }

            public Stopwatch CallTimer { get; private set; }

            public string Number { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class PhoneServiceEstablished
        {
            public string Digits { get; set; }
        }


        class PhoneStateMachine :
            MassTransitStateMachine<PrincessModelTelephone>
        {
            public PhoneStateMachine()
            {
                InstanceState(x => x.CurrentState);

                SubState(() => OnHold, Connected);

                Initially(
                    When(ServiceEstablished)
                        .Then(context => context.Instance.Number = context.Data.Digits)
                        .TransitionTo(OffHook));

                During(OffHook,
                    When(CallDialed)
                        .TransitionTo(Ringing));

                During(Ringing,
                    When(HungUp)
                        .TransitionTo(OffHook),
                    When(CallConnected)
                        .TransitionTo(Connected));

                During(Connected,
                    When(LeftMessage).TransitionTo(OffHook),
                    When(HungUp).TransitionTo(OffHook),
                    When(PlacedOnHold).TransitionTo(OnHold));

                During(OnHold,
                    When(TakenOffHold).TransitionTo(Connected),
                    When(PhoneHurledAgainstWall).TransitionTo(PhoneDestroyed));

                DuringAny(
                    When(Connected.Enter)
                        .Then(context => StartCallTimer(context.Instance)),
                    When(Connected.Leave)
                        .Then(context => StopCallTimer(context.Instance)));
            }

            public State OffHook { get; set; }
            public State Ringing { get; set; }
            public State Connected { get; set; }
            public State OnHold { get; set; }
            public State PhoneDestroyed { get; set; }

            public Event<PhoneServiceEstablished> ServiceEstablished { get; set; }
            public Event CallDialed { get; set; }
            public Event HungUp { get; set; }
            public Event CallConnected { get; set; }
            public Event LeftMessage { get; set; }
            public Event PlacedOnHold { get; set; }
            public Event TakenOffHold { get; set; }
            public Event PhoneHurledAgainstWall { get; set; }

            void StopCallTimer(PrincessModelTelephone instance)
            {
                instance.CallTimer.Stop();

                Console.WriteLine("Stopped call timer at {0}ms", instance.CallTimer.ElapsedMilliseconds);
            }

            void StartCallTimer(PrincessModelTelephone instance)
            {
                Console.WriteLine("Started call timer");

                instance.CallTimer.Start();
            }
        }
    }
}

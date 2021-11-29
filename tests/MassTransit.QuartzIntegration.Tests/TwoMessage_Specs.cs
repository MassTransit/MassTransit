namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Saga;


    [TestFixture]
    public class Scheduling_two_messages_from_the_same_saga_instance :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task Should_deliver_both_messages()
        {
            var accountId = NewId.NextGuid();

            Task<ConsumeContext<AccountOpened>> opened = await ConnectPublishHandler<AccountOpened>(x => x.Message.AccountId == accountId);
            Task<ConsumeContext<AccountDefaulted>> defaulted = await ConnectPublishHandler<AccountDefaulted>(x => x.Message.AccountId == accountId);
            Task<ConsumeContext<AccountClosed>> closed = await ConnectPublishHandler<AccountClosed>(x => x.Message.AccountId == accountId);

            await InputQueueSendEndpoint.Send<OpenAccount>(new {AccountId = accountId});

            await opened;
            await Task.Delay(1000);

            await AdvanceTime(TimeSpan.FromDays(30));

            await defaulted;

            await AdvanceTime(TimeSpan.FromDays(60));

            await closed;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Account>();

            configurator.UseInMemoryOutbox();
            configurator.StateMachineSaga(_machine, _repository);
        }

        TestStateMachine _machine;
        InMemorySagaRepository<Account> _repository;


        public interface OpenAccount
        {
            Guid AccountId { get; }
        }


        public interface AccountOpened
        {
            Guid AccountId { get; }
        }


        public interface PaymentTimeFrameElapsed
        {
            Guid AccountId { get; }
        }


        public interface GracePeriodElapsed
        {
            Guid AccountId { get; }
        }


        public interface AccountDefaulted
        {
            Guid AccountId { get; }
        }


        public interface AccountClosed
        {
            Guid AccountId { get; }
        }


        class Account :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }

            public Guid? PaymentTimeFrameToken { get; set; }
            public Guid? GracePeriodToken { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Account>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => Opened, x => x.CorrelateById(context => context.Message.AccountId));

                Schedule(() => GracePeriod, x => x.GracePeriodToken, x =>
                {
                    x.Delay = TimeSpan.FromDays(30);
                    x.Received = e => e.CorrelateById(ctx => ctx.Message.AccountId);
                });

                Schedule(() => PaymentTimeFrame, x => x.PaymentTimeFrameToken, x =>
                {
                    x.Delay = TimeSpan.FromDays(90);
                    x.Received = e => e.CorrelateById(ctx => ctx.Message.AccountId);
                });

                Initially(
                    When(Opened)
                        .Schedule(GracePeriod, context => context.Init<GracePeriodElapsed>(new {AccountId = context.Instance.CorrelationId}))
                        .Schedule(PaymentTimeFrame, context => context.Init<PaymentTimeFrameElapsed>(new {AccountId = context.Instance.CorrelationId}))
                        .PublishAsync(context => context.Init<AccountOpened>(new {AccountId = context.Instance.CorrelationId}))
                        .Then(context => Console.WriteLine("PaymentTimeFrame: {0}, GracePeriod: {1}", context.Instance.PaymentTimeFrameToken, context
                            .Instance.GracePeriodToken))
                        .TransitionTo(Open));

                During(Open,
                    When(GracePeriod.Received)
                        .PublishAsync(context => context.Init<AccountDefaulted>(new {AccountId = context.Instance.CorrelationId}))
                        .TransitionTo(Defaulted),
                    When(PaymentTimeFrame.Received)
                        .PublishAsync(context => context.Init<AccountClosed>(new {AccountId = context.Instance.CorrelationId}))
                        .Unschedule(GracePeriod)
                        .TransitionTo(Closed));

                During(Defaulted,
                    When(PaymentTimeFrame.Received)
                        .PublishAsync(context => context.Init<AccountClosed>(new {AccountId = context.Instance.CorrelationId}))
                        .TransitionTo(Closed));
            }

            public Schedule<Account, PaymentTimeFrameElapsed> PaymentTimeFrame { get; set; }
            public Schedule<Account, GracePeriodElapsed> GracePeriod { get; set; }

            public State Open { get; private set; }
            public State Closed { get; private set; }
            public State Defaulted { get; private set; }

            public Event<OpenAccount> Opened { get; private set; }
        }
    }
}

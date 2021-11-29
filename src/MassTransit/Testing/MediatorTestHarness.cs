namespace MassTransit.Testing
{
    using System;
    using System.Threading.Tasks;
    using Implementations;
    using Mediator;


    public class MediatorTestHarness :
        AsyncTestHarness
    {
        BusTestConsumeObserver _consumed;
        BusTestPublishObserver _published;
        BusTestSendObserver _sent;

        public MediatorTestHarness()
        {
            TestInactivityTimeout = TimeSpan.FromSeconds(1);
        }

        public IMediator Mediator { get; private set; }

        public event Action<IMediatorConfigurator> OnConfigureMediator;

        public virtual async Task Start()
        {
            _consumed = new BusTestConsumeObserver(TestTimeout, InactivityToken);
            _consumed.ConnectInactivityObserver(InactivityObserver);

            _published = new BusTestPublishObserver(TestTimeout, TestInactivityTimeout, InactivityToken);
            _published.ConnectInactivityObserver(InactivityObserver);

            _sent = new BusTestSendObserver(TestTimeout, TestInactivityTimeout, InactivityToken);
            _sent.ConnectInactivityObserver(InactivityObserver);

            Mediator = CreateMediator();

            Mediator.ConnectConsumeObserver(_consumed);
            Mediator.ConnectPublishObserver(_published);
            Mediator.ConnectSendObserver(_sent);
        }

        protected virtual void ConfigureMediator(IMediatorConfigurator configurator)
        {
            OnConfigureMediator?.Invoke(configurator);
        }

        public virtual IRequestClient<TRequest> CreateRequestClient<TRequest>()
            where TRequest : class
        {
            return Mediator.CreateRequestClient<TRequest>(TestTimeout);
        }

        IMediator CreateMediator()
        {
            return Bus.Factory.CreateMediator(configurator =>
            {
                ConfigureMediator(configurator);
            });
        }
    }
}

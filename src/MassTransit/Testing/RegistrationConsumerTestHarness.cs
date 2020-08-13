namespace MassTransit.Testing
{
    using MessageObservers;


    public class RegistrationConsumerTestHarness<TConsumer> :
        IConsumerTestHarness<TConsumer>
        where TConsumer : class, IConsumer
    {
        readonly ReceivedMessageList _consumed;

        public RegistrationConsumerTestHarness(ConsumerTestHarnessRegistration<TConsumer> registration)
        {
            _consumed = registration.Consumed;
        }

        public IReceivedMessageList Consumed => _consumed;
    }
}

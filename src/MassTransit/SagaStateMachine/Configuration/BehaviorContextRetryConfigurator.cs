namespace MassTransit.Configuration
{
    using Observables;


    public class BehaviorContextRetryConfigurator :
        ExceptionSpecification,
        IRetryConfigurator
    {
        readonly RetryObservable _observers;

        public BehaviorContextRetryConfigurator()
        {
            _observers = new RetryObservable();
        }

        public RetryPolicyFactory PolicyFactory { get; private set; }

        public void SetRetryPolicy(RetryPolicyFactory factory)
        {
            PolicyFactory = factory;
        }

        public ConnectHandle ConnectRetryObserver(IRetryObserver observer)
        {
            return _observers.Connect(observer);
        }

        public IRetryPolicy GetRetryPolicy()
        {
            return PolicyFactory(Filter);
        }
    }
}

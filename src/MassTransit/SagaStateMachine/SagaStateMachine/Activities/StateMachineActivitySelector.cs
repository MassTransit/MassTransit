namespace MassTransit.SagaStateMachine
{
    public class StateMachineActivitySelector<TSaga> :
        IStateMachineActivitySelector<TSaga>
        where TSaga : class, SagaStateMachineInstance
    {
        readonly EventActivityBinder<TSaga> _binder;

        public StateMachineActivitySelector(EventActivityBinder<TSaga> binder)
        {
            _binder = binder;
        }

        public EventActivityBinder<TSaga> OfType<TActivity>()
            where TActivity : class, IStateMachineActivity<TSaga>
        {
            var activity = new ContainerFactoryActivity<TSaga, TActivity>();

            return _binder.Add(activity);
        }
    }


    public class StateMachineActivitySelector<TSaga, TMessage> :
        IStateMachineActivitySelector<TSaga, TMessage>
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
    {
        readonly EventActivityBinder<TSaga, TMessage> _binder;

        public StateMachineActivitySelector(EventActivityBinder<TSaga, TMessage> binder)
        {
            _binder = binder;
        }

        public EventActivityBinder<TSaga, TMessage> OfType<TActivity>()
            where TActivity : class, IStateMachineActivity<TSaga, TMessage>
        {
            var activity = new ContainerFactoryActivity<TSaga, TMessage, TActivity>();

            return _binder.Add(activity);
        }

        public EventActivityBinder<TSaga, TMessage> OfInstanceType<TActivity>()
            where TActivity : class, IStateMachineActivity<TSaga>
        {
            var activity = new ContainerFactoryActivity<TSaga, TActivity>();

            return _binder.Add(activity);
        }
    }
}

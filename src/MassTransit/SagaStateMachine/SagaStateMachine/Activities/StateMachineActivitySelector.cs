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

        EventActivityBinder<TSaga> IStateMachineActivitySelector<TSaga>.OfType<TActivity>()
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

        EventActivityBinder<TSaga, TMessage> IStateMachineActivitySelector<TSaga, TMessage>.OfType<TActivity>()
        {
            var activity = new ContainerFactoryActivity<TSaga, TMessage, TActivity>();

            return _binder.Add(activity);
        }

        EventActivityBinder<TSaga, TMessage> IStateMachineActivitySelector<TSaga, TMessage>.OfInstanceType<TActivity>()
        {
            var activity = new ContainerFactoryActivity<TSaga, TActivity>();

            return _binder.Add(activity);
        }
    }
}

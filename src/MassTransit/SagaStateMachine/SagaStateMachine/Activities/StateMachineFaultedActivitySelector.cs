namespace MassTransit.SagaStateMachine
{
    using System;


    public class StateMachineFaultedActivitySelector<TSaga, TException> :
        IStateMachineFaultedActivitySelector<TSaga, TException>
        where TSaga : class, SagaStateMachineInstance
        where TException : Exception
    {
        readonly ExceptionActivityBinder<TSaga, TException> _binder;

        public StateMachineFaultedActivitySelector(ExceptionActivityBinder<TSaga, TException> binder)
        {
            _binder = binder;
        }

        public ExceptionActivityBinder<TSaga, TException> OfType<TActivity>()
            where TActivity : class, IStateMachineActivity<TSaga>
        {
            var activity = new FaultedContainerFactoryActivity<TSaga, TException, TActivity>();

            return _binder.Add(activity);
        }
    }


    public class StateMachineFaultedActivitySelector<TSaga, TMessage, TException> :
        IStateMachineFaultedActivitySelector<TSaga, TMessage, TException>
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
        where TException : Exception
    {
        readonly ExceptionActivityBinder<TSaga, TMessage, TException> _binder;

        public StateMachineFaultedActivitySelector(ExceptionActivityBinder<TSaga, TMessage, TException> binder)
        {
            _binder = binder;
        }

        public ExceptionActivityBinder<TSaga, TMessage, TException> OfType<TActivity>()
            where TActivity : class, IStateMachineActivity<TSaga, TMessage>
        {
            var activity = new FaultedContainerFactoryActivity<TSaga, TMessage, TException, TActivity>();

            return _binder.Add(activity);
        }

        public ExceptionActivityBinder<TSaga, TMessage, TException> OfInstanceType<TActivity>()
            where TActivity : class, IStateMachineActivity<TSaga>
        {
            var activity = new FaultedContainerFactoryActivity<TSaga, TException, TActivity>();

            return _binder.Add(activity);
        }
    }
}

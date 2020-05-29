namespace Automatonymous.Activities
{
    using Binders;


    public class StateMachineActivitySelector<TInstance> :
        IStateMachineActivitySelector<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        readonly EventActivityBinder<TInstance> _binder;

        public StateMachineActivitySelector(EventActivityBinder<TInstance> binder)
        {
            _binder = binder;
        }

        EventActivityBinder<TInstance> IStateMachineActivitySelector<TInstance>.OfType<TActivity>()
        {
            var activity = new ContainerFactoryActivity<TInstance, TActivity>();

            return _binder.Add(activity);
        }
    }


    public class StateMachineActivitySelector<TInstance, TData> :
        IStateMachineActivitySelector<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        readonly EventActivityBinder<TInstance, TData> _binder;

        public StateMachineActivitySelector(EventActivityBinder<TInstance, TData> binder)
        {
            _binder = binder;
        }

        EventActivityBinder<TInstance, TData> IStateMachineActivitySelector<TInstance, TData>.OfType<TActivity>()
        {
            var activity = new ContainerFactoryActivity<TInstance, TData, TActivity>();

            return _binder.Add(activity);
        }

        EventActivityBinder<TInstance, TData> IStateMachineActivitySelector<TInstance, TData>.OfInstanceType<TActivity>()
        {
            var activity = new ContainerFactoryActivity<TInstance, TActivity>();

            return _binder.Add(activity);
        }
    }
}

namespace Automatonymous
{
    using System;


    public class DefaultConstructorStateMachineActivityFactory :
        IStateMachineActivityFactory
    {
        public Activity<TInstance, TData> GetActivity<TActivity, TInstance, TData>(BehaviorContext<TInstance, TData> context)
            where TActivity : Activity<TInstance, TData>
        {
            return (Activity<TInstance, TData>)Activator.CreateInstance(typeof(TActivity));
        }

        public Activity<TInstance> GetActivity<TActivity, TInstance>(BehaviorContext<TInstance> context)
            where TActivity : Activity<TInstance>
        {
            return (Activity<TInstance>)Activator.CreateInstance(typeof(TActivity));
        }
    }
}
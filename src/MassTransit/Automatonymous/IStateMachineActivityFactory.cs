namespace Automatonymous
{
    public interface IStateMachineActivityFactory
    {
        /// <summary>
        /// Creates a state machine activity for the specified context
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        Activity<TInstance, TData> GetActivity<TActivity, TInstance, TData>(BehaviorContext<TInstance, TData> context)
            where TActivity : class, Activity<TInstance, TData>;

        /// <summary>
        /// Creates a state machine activity for the specified context
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TInstance"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        Activity<TInstance> GetActivity<TActivity, TInstance>(BehaviorContext<TInstance> context)
            where TActivity : class, Activity<TInstance>;
    }
}

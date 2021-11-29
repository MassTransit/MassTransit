namespace MassTransit
{
    using System;
    using SagaStateMachine;


    public static class ContainerActivityExtensions
    {
        /// <summary>
        /// Adds an activity to the state machine that is resolved from the container, rather than being initialized directly.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="binder"></param>
        /// <param name="activityFactory"></param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> Activity<TInstance, TData>(this EventActivityBinder<TInstance, TData> binder,
            Func<IStateMachineActivitySelector<TInstance, TData>, EventActivityBinder<TInstance, TData>> activityFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
        {
            var selector = new StateMachineActivitySelector<TInstance, TData>(binder);

            return activityFactory(selector);
        }

        /// <summary>
        /// Adds an activity to the state machine that is resolved from the container, rather than being initialized directly.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <param name="binder"></param>
        /// <param name="activityFactory"></param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance> Activity<TInstance>(this EventActivityBinder<TInstance> binder,
            Func<IStateMachineActivitySelector<TInstance>, EventActivityBinder<TInstance>> activityFactory)
            where TInstance : class, SagaStateMachineInstance
        {
            var selector = new StateMachineActivitySelector<TInstance>(binder);

            return activityFactory(selector);
        }
    }
}

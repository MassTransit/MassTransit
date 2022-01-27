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
        /// <param name="configure"></param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> Activity<TInstance, TData>(this EventActivityBinder<TInstance, TData> binder,
            Func<IStateMachineActivitySelector<TInstance, TData>, EventActivityBinder<TInstance, TData>> configure)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
        {
            var selector = new StateMachineActivitySelector<TInstance, TData>(binder);

            return configure(selector);
        }

        /// <summary>
        /// Adds an activity to the state machine that is resolved from the container, rather than being initialized directly.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <param name="binder"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance> Activity<TInstance>(this EventActivityBinder<TInstance> binder,
            Func<IStateMachineActivitySelector<TInstance>, EventActivityBinder<TInstance>> configure)
            where TInstance : class, SagaStateMachineInstance
        {
            var selector = new StateMachineActivitySelector<TInstance>(binder);

            return configure(selector);
        }

        /// <summary>
        /// Adds an activity to the state machine that is resolved from the container, but only handles Faulted behaviors
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TException> Activity<TInstance, TException>(this ExceptionActivityBinder<TInstance, TException> binder,
            Func<IStateMachineFaultedActivitySelector<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> configure)
            where TInstance : class, SagaStateMachineInstance
            where TException : Exception
        {
            var selector = new StateMachineFaultedActivitySelector<TInstance, TException>(binder);

            return configure(selector);
        }

        /// <summary>
        /// Adds an activity to the state machine that is resolved from the container, but only handles Faulted behaviors
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TMessage, TException> Activity<TInstance, TMessage, TException>(
            this ExceptionActivityBinder<TInstance, TMessage, TException> binder,
            Func<IStateMachineFaultedActivitySelector<TInstance, TMessage, TException>, ExceptionActivityBinder<TInstance, TMessage, TException>> configure)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            var selector = new StateMachineFaultedActivitySelector<TInstance, TMessage, TException>(binder);

            return configure(selector);
        }
    }
}

namespace MassTransit
{
    using System;


    public interface ExceptionActivityBinder<TSaga, TException> :
        EventActivities<TSaga>
        where TSaga : class, ISaga
        where TException : Exception
    {
        StateMachine<TSaga> StateMachine { get; }

        Event Event { get; }

        ExceptionActivityBinder<TSaga, TException> Add(IStateMachineActivity<TSaga> activity);

        /// <summary>
        /// Catch an exception and execute the compensating activities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TSaga, TException> Catch<T>(Func<ExceptionActivityBinder<TSaga, T>, ExceptionActivityBinder<TSaga, T>> activityCallback)
            where T : Exception;

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TSaga, TException> If(StateMachineExceptionCondition<TSaga, TException> condition,
            Func<ExceptionActivityBinder<TSaga, TException>, ExceptionActivityBinder<TSaga, TException>> activityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TSaga, TException> IfAsync(StateMachineAsyncExceptionCondition<TSaga, TException> condition,
            Func<ExceptionActivityBinder<TSaga, TException>, ExceptionActivityBinder<TSaga, TException>> activityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="thenActivityCallback"></param>
        /// <param name="elseActivityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TSaga, TException> IfElse(StateMachineExceptionCondition<TSaga, TException> condition,
            Func<ExceptionActivityBinder<TSaga, TException>, ExceptionActivityBinder<TSaga, TException>> thenActivityCallback,
            Func<ExceptionActivityBinder<TSaga, TException>, ExceptionActivityBinder<TSaga, TException>> elseActivityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="thenActivityCallback"></param>
        /// <param name="elseActivityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TSaga, TException> IfElseAsync(StateMachineAsyncExceptionCondition<TSaga, TException> condition,
            Func<ExceptionActivityBinder<TSaga, TException>, ExceptionActivityBinder<TSaga, TException>> thenActivityCallback,
            Func<ExceptionActivityBinder<TSaga, TException>, ExceptionActivityBinder<TSaga, TException>> elseActivityCallback);
    }


    public interface ExceptionActivityBinder<TSaga, TMessage, TException> :
        EventActivities<TSaga>
        where TSaga : class, ISaga
        where TException : Exception
        where TMessage : class
    {
        StateMachine<TSaga> StateMachine { get; }

        Event<TMessage> Event { get; }

        ExceptionActivityBinder<TSaga, TMessage, TException> Add(IStateMachineActivity<TSaga> activity);

        ExceptionActivityBinder<TSaga, TMessage, TException> Add(IStateMachineActivity<TSaga, TMessage> activity);

        /// <summary>
        /// Catch an exception and execute the compensating activities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TSaga, TMessage, TException> Catch<T>(
            Func<ExceptionActivityBinder<TSaga, TMessage, T>, ExceptionActivityBinder<TSaga, TMessage, T>> activityCallback)
            where T : Exception;

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TSaga, TMessage, TException> If(StateMachineExceptionCondition<TSaga, TMessage, TException> condition,
            Func<ExceptionActivityBinder<TSaga, TMessage, TException>, ExceptionActivityBinder<TSaga, TMessage, TException>> activityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TSaga, TMessage, TException> IfAsync(StateMachineAsyncExceptionCondition<TSaga, TMessage, TException> condition,
            Func<ExceptionActivityBinder<TSaga, TMessage, TException>, ExceptionActivityBinder<TSaga, TMessage, TException>> activityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="thenActivityCallback"></param>
        /// <param name="elseActivityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TSaga, TMessage, TException> IfElse(StateMachineExceptionCondition<TSaga, TMessage, TException> condition,
            Func<ExceptionActivityBinder<TSaga, TMessage, TException>, ExceptionActivityBinder<TSaga, TMessage, TException>> thenActivityCallback,
            Func<ExceptionActivityBinder<TSaga, TMessage, TException>, ExceptionActivityBinder<TSaga, TMessage, TException>> elseActivityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="thenActivityCallback"></param>
        /// <param name="elseActivityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TSaga, TMessage, TException> IfElseAsync(StateMachineAsyncExceptionCondition<TSaga, TMessage, TException> condition,
            Func<ExceptionActivityBinder<TSaga, TMessage, TException>, ExceptionActivityBinder<TSaga, TMessage, TException>> thenActivityCallback,
            Func<ExceptionActivityBinder<TSaga, TMessage, TException>, ExceptionActivityBinder<TSaga, TMessage, TException>> elseActivityCallback);
    }
}

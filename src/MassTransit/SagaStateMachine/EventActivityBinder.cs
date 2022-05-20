namespace MassTransit
{
    using System;


    public interface EventActivityBinder<TSaga> :
        EventActivities<TSaga>
        where TSaga : class, ISaga
    {
        StateMachine<TSaga> StateMachine { get; }

        Event Event { get; }

        EventActivityBinder<TSaga> Add(IStateMachineActivity<TSaga> activity);

        /// <summary>
        /// Catch the exception of type T, and execute the compensation chain
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        EventActivityBinder<TSaga> Catch<T>(Func<ExceptionActivityBinder<TSaga, T>, ExceptionActivityBinder<TSaga, T>> activityCallback)
            where T : Exception;

        /// <summary>
        /// Retry the behavior, using the specified retry policy
        /// </summary>
        /// <param name="configure">Configures the retry</param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        EventActivityBinder<TSaga> Retry(Action<IRetryConfigurator> configure,
            Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        EventActivityBinder<TSaga> If(StateMachineCondition<TSaga> condition,
            Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        EventActivityBinder<TSaga> IfAsync(StateMachineAsyncCondition<TSaga> condition,
            Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="thenActivityCallback"></param>
        /// <param name="elseActivityCallback"></param>
        /// <returns></returns>
        EventActivityBinder<TSaga> IfElse(StateMachineCondition<TSaga> condition,
            Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> thenActivityCallback,
            Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> elseActivityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="thenActivityCallback"></param>
        /// <param name="elseActivityCallback"></param>
        /// <returns></returns>
        EventActivityBinder<TSaga> IfElseAsync(StateMachineAsyncCondition<TSaga> condition,
            Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> thenActivityCallback,
            Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> elseActivityCallback);
    }


    public interface EventActivityBinder<TSaga, TMessage> :
        EventActivities<TSaga>
        where TSaga : class, ISaga
        where TMessage : class
    {
        StateMachine<TSaga> StateMachine { get; }

        Event<TMessage> Event { get; }

        EventActivityBinder<TSaga, TMessage> Add(IStateMachineActivity<TSaga> activity);

        EventActivityBinder<TSaga, TMessage> Add(IStateMachineActivity<TSaga, TMessage> activity);

        /// <summary>
        /// Catch the exception of type T, and execute the compensation chain
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        EventActivityBinder<TSaga, TMessage> Catch<T>(
            Func<ExceptionActivityBinder<TSaga, TMessage, T>, ExceptionActivityBinder<TSaga, TMessage, T>> activityCallback)
            where T : Exception;

        /// <summary>
        /// Retry the behavior, using the specified retry policy
        /// </summary>
        /// <param name="configure">Configures the retry</param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        EventActivityBinder<TSaga, TMessage> Retry(Action<IRetryConfigurator> configure,
            Func<EventActivityBinder<TSaga, TMessage>, EventActivityBinder<TSaga, TMessage>> activityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        EventActivityBinder<TSaga, TMessage> If(StateMachineCondition<TSaga, TMessage> condition,
            Func<EventActivityBinder<TSaga, TMessage>, EventActivityBinder<TSaga, TMessage>> activityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        EventActivityBinder<TSaga, TMessage> IfAsync(StateMachineAsyncCondition<TSaga, TMessage> condition,
            Func<EventActivityBinder<TSaga, TMessage>, EventActivityBinder<TSaga, TMessage>> activityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="thenActivityCallback"></param>
        /// <param name="elseActivityCallback"></param>
        /// <returns></returns>
        EventActivityBinder<TSaga, TMessage> IfElse(StateMachineCondition<TSaga, TMessage> condition,
            Func<EventActivityBinder<TSaga, TMessage>, EventActivityBinder<TSaga, TMessage>> thenActivityCallback,
            Func<EventActivityBinder<TSaga, TMessage>, EventActivityBinder<TSaga, TMessage>> elseActivityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="thenActivityCallback"></param>
        /// <param name="elseActivityCallback"></param>
        /// <returns></returns>
        EventActivityBinder<TSaga, TMessage> IfElseAsync(StateMachineAsyncCondition<TSaga, TMessage> condition,
            Func<EventActivityBinder<TSaga, TMessage>, EventActivityBinder<TSaga, TMessage>> thenActivityCallback,
            Func<EventActivityBinder<TSaga, TMessage>, EventActivityBinder<TSaga, TMessage>> elseActivityCallback);
    }
}

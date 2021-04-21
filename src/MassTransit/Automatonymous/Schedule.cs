namespace Automatonymous
{
    using System;


    /// <summary>
    /// Holds the state of a scheduled message
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public interface Schedule<in TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        /// <summary>
        /// The name of the scheduled message
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns the delay, given the instance, for the scheduled message
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        TimeSpan GetDelay(ConsumeEventContext<TInstance> context);

        /// <summary>
        /// Return the TokenId for the instance
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        Guid? GetTokenId(TInstance instance);

        /// <summary>
        /// Set the token ID on the Instance
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="tokenId"></param>
        void SetTokenId(TInstance instance, Guid? tokenId);
    }


    /// <summary>
    /// Holds the state of a scheduled message
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public interface Schedule<in TInstance, TMessage> :
        Schedule<TInstance>
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class
    {
        /// <summary>
        /// This event is raised when the scheduled message is received. If a previous message
        /// was rescheduled, this event is filtered so that only the most recently scheduled
        /// message is allowed.
        /// </summary>
        Event<TMessage> Received { get; set; }

        /// <summary>
        /// This event is raised when any message is directed at the state machine, but it is
        /// not filtered to the currently scheduled event. So outdated or original events may
        /// be raised.
        /// </summary>
        Event<TMessage> AnyReceived { get; set; }
    }
}

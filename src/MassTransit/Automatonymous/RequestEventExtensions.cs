namespace Automatonymous
{
    using Activities;
    using Binders;
    using Contracts;
    using MassTransit.Contracts;


    public static class RequestEventExtensions
    {
        /// <summary>
        /// Publishes the <see cref="Automatonymous.Contracts.RequestStarted"/> event, used by the request state machine to track
        /// pending requests for a saga instance.
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> RequestStarted<TInstance, TData>(this EventActivityBinder<TInstance, TData> source)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
        {
            return source.Add(new RequestStartedActivity<TInstance, TData>());
        }

        /// <summary>
        /// Publishes the <see cref="Automatonymous.Contracts.RequestCompleted{TResponse}"/> event, used by the request state machine to complete pending
        /// requests. The response type of the inbound request must be the same as the <typeparamref name="TData"/> type.
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> RequestCompleted<TInstance, TData>(this EventActivityBinder<TInstance, TData> source)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
        {
            return source.Add(new RequestCompletedActivity<TInstance, TData>());
        }

        /// <summary>
        /// Publishes the <see cref="Automatonymous.Contracts.RequestCompleted{TResponse}"/> event, used by the request state machine to complete pending
        /// requests.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="messageFactory"></param>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> RequestCompleted<TInstance, TData, TResponse>(this EventActivityBinder<TInstance, TData> source,
            AsyncEventMessageFactory<TInstance, TData, TResponse> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TResponse : class
        {
            return source.Add(new RequestCompletedActivity<TInstance, TData, TResponse>(messageFactory));
        }

        /// <summary>
        /// Publishes the <see cref="Automatonymous.Contracts.RequestFaulted{T}"/> event, used by the request state machine to fault pending requests
        /// </summary>
        /// <param name="source"></param>
        /// <param name="requestEvent"></param>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TRequest"></typeparam>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> RequestFaulted<TInstance, TData, TRequest>(this EventActivityBinder<TInstance, TData> source,
            Event<TRequest> requestEvent)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TRequest : class
        {
            return source.Add(new RequestFaultedActivity<TInstance, TData, TRequest>());
        }
    }
}

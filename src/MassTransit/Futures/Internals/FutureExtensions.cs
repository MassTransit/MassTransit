namespace MassTransit.Futures.Internals
{
    using System;
    using Automatonymous;
    using Automatonymous.Binders;
    using Courier.Contracts;
    using MassTransit;


    public static class FutureExtensions
    {
        /// <summary>
        /// Initialize the FutureState properties of the request
        /// </summary>
        /// <param name="binder"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static EventActivityBinder<FutureState, T> InitializeFuture<T>(this EventActivityBinder<FutureState, T> binder)
            where T : class
        {
            return binder
                .Then(context =>
                {
                    FutureConsumeContext<T> consumeContext = context.CreateFutureConsumeContext(context.Data);

                    context.Instance.Created = DateTime.UtcNow;
                    context.Instance.Command = new FutureMessage<T>(consumeContext.Message);
                    context.Instance.Location = new FutureLocation(context.Instance.CorrelationId, consumeContext.ReceiveContext.InputAddress);

                    consumeContext.AddSubscription();
                });
        }

        /// <summary>
        /// Use when a request is received after the initial request is still awaiting completion
        /// </summary>
        /// <param name="binder"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static EventActivityBinder<FutureState, T> AddSubscription<T>(this EventActivityBinder<FutureState, T> binder)
            where T : class
        {
            return binder.Then(context =>
            {
                FutureConsumeContext<T> consumeContext = context.CreateFutureConsumeContext(context.Data);

                consumeContext.AddSubscription();
            });
        }

        /// <summary>
        /// Set the result associated with the identifier using the message factory
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="getResultId">Should return the result identifier</param>
        /// <param name="messageFactory">Should return the result message</param>
        /// <typeparam name="T">The event type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <returns></returns>
        public static EventActivityBinder<FutureState, T> SetResult<T, TResult>(this EventActivityBinder<FutureState, T> binder,
            Func<FutureConsumeContext<T>, Guid> getResultId, AsyncFutureMessageFactory<T, TResult> messageFactory)
            where T : class
            where TResult : class
        {
            return binder.ThenAsync(context =>
            {
                FutureConsumeContext<T> consumeContext = context.CreateFutureConsumeContext();

                var resultId = getResultId(consumeContext);

                return consumeContext.SetResult(resultId, messageFactory);
            });
        }

        /// <summary>
        /// Set the result associated with the identifier using the message factory
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="getResultId">Should return the result identifier</param>
        /// <param name="messageFactory">Should return the result message</param>
        /// <typeparam name="T">The event type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <returns></returns>
        public static EventActivityBinder<FutureState, T> SetResult<T, TResult>(this EventActivityBinder<FutureState, T> binder,
            Func<FutureConsumeContext<T>, Guid> getResultId, FutureMessageFactory<T, TResult> messageFactory)
            where T : class
            where TResult : class
        {
            return binder.Then(context =>
            {
                FutureConsumeContext<T> consumeContext = context.CreateFutureConsumeContext();

                var resultId = getResultId(consumeContext);

                consumeContext.SetResult(resultId, messageFactory);
            });
        }

        /// <summary>
        /// Set the result associated with the identifier using the message factory
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="getResultId">Should return the result identifier</param>
        /// <param name="messageFactory">Should return the result message</param>
        /// <typeparam name="T">The event type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <returns></returns>
        public static EventActivityBinder<FutureState, Fault<T>> SetFault<T, TResult>(this EventActivityBinder<FutureState, Fault<T>> binder,
            Func<FutureConsumeContext<Fault<T>>, Guid> getResultId, FutureMessageFactory<Fault<T>, TResult> messageFactory)
            where T : class
            where TResult : class
        {
            return binder.Then(context =>
            {
                FutureConsumeContext<Fault<T>> consumeContext = context.CreateFutureConsumeContext();

                var resultId = getResultId(consumeContext);

                consumeContext.SetFault(resultId, messageFactory);
            });
        }

        /// <summary>
        /// Set the result associated with the identifier using the message factory
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="messageFactory">Should return the result message</param>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <returns></returns>
        public static EventActivityBinder<FutureState, RoutingSlipFaulted> SetFault<TResult>(this EventActivityBinder<FutureState, RoutingSlipFaulted> binder,
            FutureMessageFactory<RoutingSlipFaulted, TResult> messageFactory)
            where TResult : class
        {
            return binder.Then(context =>
            {
                FutureConsumeContext<RoutingSlipFaulted> consumeContext = context.CreateFutureConsumeContext();

                var resultId = consumeContext.Message.TrackingNumber;

                consumeContext.SetFault(resultId, messageFactory);
            });
        }
    }
}

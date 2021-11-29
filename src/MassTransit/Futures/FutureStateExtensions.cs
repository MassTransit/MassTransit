namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public static class FutureStateExtensions
    {
        public static T GetCommand<T>(this BehaviorContext<FutureState> context)
            where T : class
        {
            return context.Saga.Command != null && context.Saga.Command.HasMessageType<T>()
                ? context.SerializerContext.DeserializeObject<T>(context.Saga.Command.Message)
                : null;
        }

        public static T ToObject<T>(this BehaviorContext<FutureState> context, FutureMessage message)
            where T : class
        {
            return message != null && message.HasMessageType<T>()
                ? context.SerializerContext.DeserializeObject<T>(message.Message)
                : null;
        }

        public static FutureMessage CreateFutureMessage<T>(this BehaviorContext<FutureState> context, T message)
            where T : class
        {
            IDictionary<string, object> dictionary = context.SerializerContext.ToDictionary(message);

            return new FutureMessage(dictionary, MessageTypeCache<T>.MessageTypeNames);
        }

        public static IEnumerable<T> SelectResults<T>(this BehaviorContext<FutureState> context)
            where T : class
        {
            return context.Saga.HasResults()
                ? context.Saga.Results.Select(x => context.ToObject<T>(x.Value)).Where(x => x != null)
                : Enumerable.Empty<T>();
        }

        public static void AddSubscription(this BehaviorContext<FutureState> context)
        {
            if (context.ResponseAddress == null)
                return;

            context.Saga.Subscriptions.Add(new FutureSubscription(context.ResponseAddress, context.RequestId));
        }

        public static async Task<TResult> SetResult<T, TResult>(this BehaviorContext<FutureState, T> context, Guid id,
            AsyncEventMessageFactory<FutureState, T, TResult> factory)
            where T : class
            where TResult : class
        {
            if (!context.Saga.Completed.HasValue)
                SetCompleted(context, id);

            var result = await factory(context).ConfigureAwait(false);

            context.Saga.Results[id] = context.CreateFutureMessage(result);

            return result;
        }

        public static async Task<TResult> SetResult<TResult>(this BehaviorContext<FutureState> context, Guid id,
            AsyncEventMessageFactory<FutureState, TResult> factory)
            where TResult : class
        {
            if (!context.Saga.Completed.HasValue)
                SetCompleted(context, id);

            var result = await factory(context).ConfigureAwait(false);

            context.Saga.Results[id] = context.CreateFutureMessage(result);

            return result;
        }

        public static void SetResult<T, TResult>(this BehaviorContext<FutureState, T> context, Guid id, EventMessageFactory<FutureState, T, TResult> factory)
            where T : class
            where TResult : class
        {
            if (!context.Saga.Completed.HasValue)
                SetCompleted(context, id);

            var result = factory(context);

            context.Saga.Results[id] = context.CreateFutureMessage(result);
        }

        public static TResult SetResult<TResult>(this BehaviorContext<FutureState> context, Guid id, EventMessageFactory<FutureState, TResult> factory)
            where TResult : class
        {
            if (!context.Saga.Completed.HasValue)
                SetCompleted(context, id);

            var result = factory(context);

            context.Saga.Results[id] = context.CreateFutureMessage(result);

            return result;
        }

        public static void SetResult<TResult>(this BehaviorContext<FutureState> context, Guid id, TResult result)
            where TResult : class
        {
            if (!context.Saga.Completed.HasValue)
                SetCompleted(context, id);

            context.Saga.Results[id] = context.CreateFutureMessage(result);
        }

        public static void SetCompleted(this BehaviorContext<FutureState> context, Guid id)
        {
            var timestamp = context.SentTime ?? DateTime.UtcNow;

            var future = context.Saga;

            if (future.HasPending())
            {
                future.Pending.Remove(id);

                if (!future.HasPending() && !future.HasFaults())
                    future.Completed = timestamp;
            }
            else if (!future.HasFaults())
                future.Completed = timestamp;
        }

        public static void SetFaulted(this BehaviorContext<FutureState> context, Guid id, DateTime? timestamp = default)
        {
            timestamp ??= context.SentTime ?? DateTime.UtcNow;

            var future = context.Saga;

            if (future.HasPending())
                future.Pending?.Remove(id);

            future.Faulted ??= timestamp;
        }

        public static void SetFault<TFault>(this BehaviorContext<FutureState> context, Guid id, TFault fault, DateTime? timestamp = default)
            where TFault : class
        {
            SetFaulted(context, id, timestamp);

            context.Saga.Faults[id] = context.CreateFutureMessage(fault);
        }

        public static void SetFault<T, TFault>(this BehaviorContext<FutureState, T> context, Guid id, EventMessageFactory<FutureState, T, TFault> factory)
            where T : class
            where TFault : class
        {
            SetFaulted(context, id);

            var result = factory(context);

            context.Saga.Faults[id] = context.CreateFutureMessage(result);
        }

        public static async Task<TFault> SetFault<T, TFault>(this FutureState future, BehaviorContext<FutureState, T> context, Guid id,
            AsyncEventMessageFactory<FutureState, T, TFault> factory)
            where T : class
            where TFault : class
        {
            var timestamp = context.SentTime ?? DateTime.UtcNow;

            if (future.HasPending())
                future.Pending?.Remove(id);

            future.Faulted ??= timestamp;

            var fault = await factory(context).ConfigureAwait(false);

            future.Faults[id] = context.CreateFutureMessage(fault);

            return fault;
        }

        public static bool TryGetResult<T>(this BehaviorContext<FutureState> context, Guid id, out T result)
            where T : class
        {
            if (context.Saga.HasResults() && context.Saga.Results.TryGetValue(id, out var message))
            {
                result = context.ToObject<T>(message);
                return result != default;
            }

            result = default;
            return false;
        }

        public static bool TryGetFault<T>(this BehaviorContext<FutureState> context, Guid id, out T fault)
            where T : class
        {
            if (context.Saga.HasFaults() && context.Saga.Faults.TryGetValue(id, out var message))
            {
                fault = context.ToObject<T>(message);
                return fault != default;
            }

            fault = default;
            return false;
        }
    }
}

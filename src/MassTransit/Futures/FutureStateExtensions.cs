namespace MassTransit.Futures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Automatonymous;


    public static class FutureStateExtensions
    {
        public static T GetCommand<T>(this FutureState future)
            where T : class
        {
            return future.Command?.ToObject<T>();
        }

        public static IEnumerable<T> SelectResults<T>(this FutureConsumeContext context)
            where T : class
        {
            return context.Instance.HasResults()
                ? context.Instance.Results.Where(x => x.Value.HasMessageType<T>()).Select(x => x.Value.ToObject<T>())
                : Enumerable.Empty<T>();
        }

        public static void AddSubscription(this FutureConsumeContext context)
        {
            if (context.ResponseAddress == null)
                return;

            context.Instance.Subscriptions.Add(new FutureSubscription(context.ResponseAddress, context.RequestId));
        }

        public static async Task<TResult> SetResult<T, TResult>(this FutureConsumeContext<T> context, Guid id, AsyncFutureMessageFactory<T, TResult> factory)
            where T : class
            where TResult : class
        {
            if (!context.Instance.Completed.HasValue)
                SetCompleted(context, id);

            var result = await factory(context).ConfigureAwait(false);

            context.Instance.Results[id] = new FutureMessage<TResult>(result);

            return result;
        }

        public static async Task<TResult> SetResult<TResult>(this FutureConsumeContext context, Guid id, AsyncFutureMessageFactory<TResult> factory)
            where TResult : class
        {
            if (!context.Instance.Completed.HasValue)
                SetCompleted(context, id);

            var result = await factory(context).ConfigureAwait(false);

            context.Instance.Results[id] = new FutureMessage<TResult>(result);

            return result;
        }

        public static void SetResult<T, TResult>(this FutureConsumeContext<T> context, Guid id, FutureMessageFactory<T, TResult> factory)
            where T : class
            where TResult : class
        {
            if (!context.Instance.Completed.HasValue)
                SetCompleted(context, id);

            var result = factory(context);

            context.Instance.Results[id] = new FutureMessage<TResult>(result);
        }

        public static TResult SetResult<TResult>(this FutureConsumeContext context, Guid id, FutureMessageFactory<TResult> factory)
            where TResult : class
        {
            if (!context.Instance.Completed.HasValue)
                SetCompleted(context, id);

            var result = factory(context);

            context.Instance.Results[id] = new FutureMessage<TResult>(result);

            return result;
        }

        public static void SetResult<TResult>(this FutureConsumeContext context, Guid id, TResult result)
            where TResult : class
        {
            if (!context.Instance.Completed.HasValue)
                SetCompleted(context, id);

            context.Instance.Results[id] = new FutureMessage<TResult>(result);
        }

        public static void SetCompleted(this FutureConsumeContext context, Guid id)
        {
            var timestamp = context.SentTime ?? DateTime.UtcNow;

            var future = context.Instance;

            if (future.HasPending())
            {
                future.Pending.Remove(id);

                if (!future.HasPending() && !future.HasFaults())
                    future.Completed = timestamp;
            }
            else if (!future.HasFaults())
                future.Completed = timestamp;
        }

        public static void SetFaulted(this FutureConsumeContext context, Guid id, DateTime? timestamp = default)
        {
            timestamp ??= context.SentTime ?? DateTime.UtcNow;

            var future = context.Instance;

            if (future.HasPending())
                future.Pending?.Remove(id);

            future.Faulted ??= timestamp;
        }

        public static void SetFault<TFault>(this FutureConsumeContext context, Guid id, TFault fault, DateTime? timestamp = default)
            where TFault : class
        {
            SetFaulted(context, id, timestamp);

            context.Instance.Faults[id] = new FutureMessage<TFault>(fault);
        }

        public static void SetFault<T, TFault>(this FutureConsumeContext<T> context, Guid id, FutureMessageFactory<T, TFault> factory)
            where T : class
            where TFault : class
        {
            SetFaulted(context, id);

            var result = factory(context);

            context.Instance.Faults[id] = new FutureMessage<TFault>(result);
        }

        public static async Task<TFault> SetFault<T, TFault>(this FutureState future,
            ConsumeEventContext<FutureState, T> context, Guid id, AsyncEventMessageFactory<FutureState, T, TFault> factory)
            where T : class
            where TFault : class
        {
            var timestamp = context.SentTime ?? DateTime.UtcNow;

            if (future.HasPending())
                future.Pending?.Remove(id);

            future.Faulted ??= timestamp;

            var fault = await factory(context).ConfigureAwait(false);

            future.Faults[id] = new FutureMessage<TFault>(fault);

            return fault;
        }

        public static bool TryGetResult<T>(this FutureState future, Guid id, out T result)
            where T : class
        {
            if (future.HasResults() && future.Results.TryGetValue(id, out var message))
            {
                result = message.ToObject<T>();
                return result != default;
            }

            result = default;
            return false;
        }

        public static bool TryGetFault<T>(this FutureState future, Guid id, out T fault)
            where T : class
        {
            if (future.HasFaults() && future.Faults.TryGetValue(id, out var message))
            {
                fault = message.ToObject<T>();
                return fault != default;
            }

            fault = default;
            return false;
        }
    }
}

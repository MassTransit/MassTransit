namespace MassTransit.Futures
{
    using System.Threading.Tasks;
    using Automatonymous.Activities;
    using Automatonymous.Binders;
    using Internals;


    public static class FutureVariableExtensions
    {
        public static async Task<TValue> SetVariable<T, TValue>(this FutureConsumeContext<T> context, string key, AsyncFutureMessageFactory<T, TValue> factory)
            where T : class
            where TValue : class
        {
            var value = await factory(context).ConfigureAwait(false);

            context.Instance.Variables[key] = value;

            return value;
        }

        public static async Task<TValue> SetVariable<TValue>(this FutureConsumeContext context, string key, AsyncFutureMessageFactory<TValue> factory)
            where TValue : class
        {
            var value = await factory(context).ConfigureAwait(false);

            context.Instance.Variables[key] = value;

            return value;
        }

        public static TValue SetVariable<T, TValue>(this FutureConsumeContext<T> context, string key, FutureMessageFactory<T, TValue> factory)
            where T : class
            where TValue : class
        {
            var value = factory(context);

            context.Instance.Variables[key] = value;

            return value;
        }

        public static TValue SetVariable<TValue>(this FutureConsumeContext context, string key, FutureMessageFactory<TValue> factory)
            where TValue : class
        {
            var value = factory(context);

            context.Instance.Variables[key] = value;

            return value;
        }

        public static EventActivityBinder<FutureState, TData> SetVariable<TData, TValue>(this EventActivityBinder<FutureState, TData> binder, string key,
            FutureMessageFactory<TData, TValue> valueFactory)
            where TData : class
            where TValue : class
        {
            return binder.Add(new ActionActivity<FutureState, TData>(context =>
            {
                FutureConsumeContext<TData> futureContext = context.CreateFutureConsumeContext(context.Data);

                futureContext.SetVariable(key, valueFactory);
            }));
        }

        public static EventActivityBinder<FutureState> SetVariable<TValue>(this EventActivityBinder<FutureState> binder, string key,
            FutureMessageFactory<TValue> valueFactory)
            where TValue : class
        {
            return binder.Add(new ActionActivity<FutureState>(context =>
            {
                var futureContext = context.CreateFutureConsumeContext();

                futureContext.SetVariable(key, valueFactory);
            }));
        }

        public static EventActivityBinder<FutureState, TData> SetVariable<TData, TValue>(this EventActivityBinder<FutureState, TData> binder, string key,
            AsyncFutureMessageFactory<TData, TValue> valueFactory)
            where TData : class
            where TValue : class
        {
            return binder.Add(new AsyncActivity<FutureState, TData>(context =>
            {
                FutureConsumeContext<TData> futureContext = context.CreateFutureConsumeContext(context.Data);

                return futureContext.SetVariable(key, valueFactory);
            }));
        }

        public static EventActivityBinder<FutureState> SetVariable<TData, TValue>(this EventActivityBinder<FutureState> binder, string key,
            AsyncFutureMessageFactory<TValue> valueFactory)
            where TData : class
            where TValue : class
        {
            return binder.Add(new AsyncActivity<FutureState>(context =>
            {
                var futureContext = context.CreateFutureConsumeContext();

                return futureContext.SetVariable(key, valueFactory);
            }));
        }

        public static void SetVariable<TValue>(this FutureConsumeContext context, string key, TValue value)
            where TValue : class
        {
            context.Instance.Variables[key] = value;
        }

        public static bool TryGetVariable<T>(this FutureConsumeContext future, string key, out T result)
            where T : class
        {
            if (future.Instance.HasVariables())
                return future.Instance.Variables.TryGetValue(key, out result);

            result = default;
            return false;
        }
    }
}

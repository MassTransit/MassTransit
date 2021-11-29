namespace MassTransit
{
    using System.Threading.Tasks;
    using SagaStateMachine;
    using Serialization;


    public static class FutureVariableExtensions
    {
        public static async Task<TValue> SetVariable<T, TValue>(this BehaviorContext<FutureState, T> context, string key,
            AsyncEventMessageFactory<FutureState, T, TValue> factory)
            where T : class
            where TValue : class
        {
            var value = await factory(context).ConfigureAwait(false);

            context.Saga.Variables[key] = value;

            return value;
        }

        public static async Task<TValue> SetVariable<TValue>(this BehaviorContext<FutureState> context, string key,
            AsyncEventMessageFactory<FutureState, TValue> factory)
            where TValue : class
        {
            var value = await factory(context).ConfigureAwait(false);

            context.Saga.Variables[key] = value;

            return value;
        }

        public static TValue SetVariable<T, TValue>(this BehaviorContext<FutureState, T> context, string key,
            EventMessageFactory<FutureState, T, TValue> factory)
            where T : class
            where TValue : class
        {
            var value = factory(context);

            context.Saga.Variables[key] = value;

            return value;
        }

        public static TValue SetVariable<TValue>(this BehaviorContext<FutureState> context, string key, EventMessageFactory<FutureState, TValue> factory)
            where TValue : class
        {
            var value = factory(context);

            context.Saga.Variables[key] = value;

            return value;
        }

        public static EventActivityBinder<FutureState, TData> SetVariable<TData, TValue>(this EventActivityBinder<FutureState, TData> binder, string key,
            EventMessageFactory<FutureState, TData, TValue> valueFactory)
            where TData : class
            where TValue : class
        {
            return binder.Add(new ActionActivity<FutureState, TData>(context => context.SetVariable(key, valueFactory)));
        }

        public static EventActivityBinder<FutureState> SetVariable<TValue>(this EventActivityBinder<FutureState> binder, string key,
            EventMessageFactory<FutureState, TValue> valueFactory)
            where TValue : class
        {
            return binder.Add(new ActionActivity<FutureState>(context => context.SetVariable(key, valueFactory)));
        }

        public static EventActivityBinder<FutureState, TData> SetVariable<TData, TValue>(this EventActivityBinder<FutureState, TData> binder, string key,
            AsyncEventMessageFactory<FutureState, TData, TValue> valueFactory)
            where TData : class
            where TValue : class
        {
            return binder.Add(new AsyncActivity<FutureState, TData>(context => context.SetVariable(key, valueFactory)));
        }

        public static EventActivityBinder<FutureState> SetVariable<TValue>(this EventActivityBinder<FutureState> binder, string key,
            AsyncEventMessageFactory<FutureState, TValue> valueFactory)
            where TValue : class
        {
            return binder.Add(new AsyncActivity<FutureState>(context => context.SetVariable(key, valueFactory)));
        }

        public static void SetVariable<TValue>(this BehaviorContext<FutureState> context, string key, TValue value)
            where TValue : class
        {
            context.Saga.Variables[key] = value;
        }

        public static bool TryGetVariable<T>(this BehaviorContext<FutureState> context, string key, out T result)
            where T : class
        {
            if (context.Saga.HasVariables())
                return context.SerializerContext.TryGetValue(context.Saga.Variables, key, out result);

            result = default;
            return false;
        }
    }
}

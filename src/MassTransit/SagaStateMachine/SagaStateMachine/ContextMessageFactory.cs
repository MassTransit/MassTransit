namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class ContextMessageFactory<TContext, T>
        where TContext : class, ConsumeContext
        where T : class
    {
        readonly Func<TContext, Task<SendTuple<T>>> _messageFactory;

        public ContextMessageFactory(Func<TContext, Task<SendTuple<T>>> messageFactory)
        {
            _messageFactory = messageFactory;
        }

        public Task<SendTuple<T>> GetMessage(TContext context)
        {
            Task<SendTuple<T>> result = _messageFactory(context);
            if (result.Status == TaskStatus.RanToCompletion)
                return result;

            async Task<SendTuple<T>> GetResult()
            {
                return await result.ConfigureAwait(false);
            }

            return GetResult();
        }

        public Task Use(TContext context, Func<TContext, SendTuple<T>, Task> callback)
        {
            Task<SendTuple<T>> msgTask = GetMessage(context);
            if (msgTask.Status == TaskStatus.RanToCompletion)
                return callback(context, msgTask.GetAwaiter().GetResult());

            async Task GetResult()
            {
                SendTuple<T> send = await msgTask.ConfigureAwait(false);

                await callback(context, send).ConfigureAwait(false);
            }

            return GetResult();
        }

        public Task<TResult> Use<TResult>(TContext context, Func<TContext, SendTuple<T>, Task<TResult>> callback)
        {
            Task<SendTuple<T>> msgTask = GetMessage(context);
            if (msgTask.Status == TaskStatus.RanToCompletion)
                return callback(context, msgTask.GetAwaiter().GetResult());

            async Task<TResult> GetResult()
            {
                SendTuple<T> send = await msgTask.ConfigureAwait(false);

                return await callback(context, send).ConfigureAwait(false);
            }

            return GetResult();
        }

        public static implicit operator ContextMessageFactory<TContext, T>(TaskMessageFactory<T> factory)
        {
            Task<SendTuple<T>> message = factory.GetMessage();

            return new ContextMessageFactory<TContext, T>(context => message);
        }
    }
}

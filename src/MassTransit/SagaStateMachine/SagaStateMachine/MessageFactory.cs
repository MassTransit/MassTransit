namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public static class MessageFactory<T>
        where T : class
    {
        public static TaskMessageFactory<T> Create(T message)
        {
            return new TaskMessageFactory<T>(Task.FromResult(new SendTuple<T>(message)));
        }

        public static TaskMessageFactory<T> Create(T message, IPipe<SendContext<T>> pipe)
        {
            return pipe.IsNotEmpty()
                ? new TaskMessageFactory<T>(Task.FromResult(new SendTuple<T>(message, pipe)))
                : Create(message);
        }

        public static TaskMessageFactory<T> Create(T message, Action<SendContext<T>> callback)
        {
            if (callback == null)
                return Create(message);

            IPipe<SendContext<T>> callbackPipe = Pipe.Execute(callback);

            return new TaskMessageFactory<T>(Task.FromResult(new SendTuple<T>(message, callbackPipe)));
        }

        public static TaskMessageFactory<T> Create(Task<T> factory)
        {
            if (factory.Status == TaskStatus.RanToCompletion)
                return new TaskMessageFactory<T>(Task.FromResult(new SendTuple<T>(factory.GetAwaiter().GetResult())));

            async Task<SendTuple<T>> Factory()
            {
                return new SendTuple<T>(await factory.ConfigureAwait(false));
            }

            return new TaskMessageFactory<T>(Factory());
        }

        public static TaskMessageFactory<T> Create(Task<T> factory, IPipe<SendContext<T>> pipe)
        {
            if (!pipe.IsNotEmpty())
                return Create(factory);

            if (factory.Status == TaskStatus.RanToCompletion)
                return new TaskMessageFactory<T>(Task.FromResult(new SendTuple<T>(factory.GetAwaiter().GetResult(), pipe)));

            async Task<SendTuple<T>> Factory()
            {
                return new SendTuple<T>(await factory.ConfigureAwait(false), pipe);
            }

            return new TaskMessageFactory<T>(Factory());
        }

        public static TaskMessageFactory<T> Create(Task<T> factory, Action<SendContext<T>> callback)
        {
            if (callback == null)
                return Create(factory);

            IPipe<SendContext<T>> callbackPipe = Pipe.Execute(callback);

            if (factory.Status == TaskStatus.RanToCompletion)
                return new TaskMessageFactory<T>(Task.FromResult(new SendTuple<T>(factory.GetAwaiter().GetResult(), callbackPipe)));

            async Task<SendTuple<T>> Factory()
            {
                return new SendTuple<T>(await factory.ConfigureAwait(false), callbackPipe);
            }

            return new TaskMessageFactory<T>(Factory());
        }

        // Saga/Message

        public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(T message,
            SendContextCallback<TSaga, TMessage, T> callback)
            where TSaga : class, ISaga
            where TMessage : class
        {
            return callback == null
                ? Create(message)
                : Create(context => message, callback);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(Task<T> factory,
            SendContextCallback<TSaga, TMessage, T> callback)
            where TSaga : class, ISaga
            where TMessage : class
        {
            return callback == null
                ? Create(factory)
                : Create(context => factory, callback);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(
            Func<BehaviorContext<TSaga, TMessage>, Task<SendTuple<T>>> factory)
            where TSaga : class, ISaga
            where TMessage : class
        {
            return new ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T>(factory);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(
            Func<BehaviorContext<TSaga, TMessage>, Task<SendTuple<T>>> factory, Action<SendContext<T>> callback)
            where TSaga : class, ISaga
            where TMessage : class
        {
            if (callback == null)
                return Create(factory);

            async Task<SendTuple<T>> Factory(BehaviorContext<TSaga, TMessage> context)
            {
                (var message, IPipe<SendContext<T>> sendPipe) = await factory(context).ConfigureAwait(false);
                if (sendPipe.IsNotEmpty())
                {
                    IPipe<SendContext<T>> pipe = sendPipe.AddCallback(callback);
                    return new SendTuple<T>(message, pipe);
                }

                return new SendTuple<T>(message, Pipe.Execute(callback));
            }

            return new ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(
            Func<BehaviorContext<TSaga, TMessage>, Task<SendTuple<T>>> factory, SendContextCallback<TSaga, TMessage, T> callback)
            where TSaga : class, ISaga
            where TMessage : class
        {
            if (callback == null)
                return Create(factory);

            async Task<SendTuple<T>> Factory(BehaviorContext<TSaga, TMessage> context)
            {
                SendTuple<T> result = await factory(context).ConfigureAwait(false);
                if (result.Pipe.IsNotEmpty())
                {
                    IPipe<SendContext<T>> pipe = result.Pipe.AddCallback(ctx => callback(context, ctx));
                    return new SendTuple<T>(result.Message, pipe);
                }

                return new SendTuple<T>(result.Message, Pipe.Execute<SendContext<T>>(ctx => callback(context, ctx)));
            }

            return new ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(AsyncEventMessageFactory<TSaga, TMessage, T> factory)
            where TSaga : class, ISaga
            where TMessage : class
        {
            Task<SendTuple<T>> Factory(BehaviorContext<TSaga, TMessage> context)
            {
                Task<T> result = factory(context);
                if (result.Status == TaskStatus.RanToCompletion)
                    return Task.FromResult(new SendTuple<T>(result.GetAwaiter().GetResult()));

                async Task<SendTuple<T>> GetResult()
                {
                    return new SendTuple<T>(await result.ConfigureAwait(false));
                }

                return GetResult();
            }

            return new ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(AsyncEventMessageFactory<TSaga, TMessage, T> factory,
            IPipe<SendContext<T>> pipe)
            where TSaga : class, ISaga
            where TMessage : class
        {
            if (!pipe.IsNotEmpty())
                return Create(factory);

            Task<SendTuple<T>> Factory(BehaviorContext<TSaga, TMessage> context)
            {
                Task<T> result = factory(context);
                if (result.Status == TaskStatus.RanToCompletion)
                    return Task.FromResult(new SendTuple<T>(result.GetAwaiter().GetResult(), pipe));

                async Task<SendTuple<T>> GetResult()
                {
                    return new SendTuple<T>(await result.ConfigureAwait(false), pipe);
                }

                return GetResult();
            }

            return new ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(AsyncEventMessageFactory<TSaga, TMessage, T> factory,
            Action<SendContext<T>> callback)
            where TSaga : class, ISaga
            where TMessage : class
        {
            return callback == null ? Create(factory) : Create(factory, Pipe.Execute(callback));
        }

        public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(AsyncEventMessageFactory<TSaga, TMessage, T> factory,
            SendContextCallback<TSaga, TMessage, T> callback)
            where TSaga : class, ISaga
            where TMessage : class
        {
            if (callback == null)
                return Create(factory);

            Task<SendTuple<T>> Factory(BehaviorContext<TSaga, TMessage> context)
            {
                IPipe<SendContext<T>> callbackPipe = Pipe.Execute<SendContext<T>>(ctx => callback(context, ctx));

                Task<T> result = factory(context);
                if (result.Status == TaskStatus.RanToCompletion)
                    return Task.FromResult(new SendTuple<T>(result.GetAwaiter().GetResult(), callbackPipe));

                async Task<SendTuple<T>> GetResult()
                {
                    return new SendTuple<T>(await result.ConfigureAwait(false), callbackPipe);
                }

                return GetResult();
            }

            return new ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(EventMessageFactory<TSaga, TMessage, T> factory)
            where TSaga : class, ISaga
            where TMessage : class
        {
            Task<SendTuple<T>> Factory(BehaviorContext<TSaga, TMessage> context)
            {
                var result = factory(context);
                return Task.FromResult(new SendTuple<T>(result));
            }

            return new ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(EventMessageFactory<TSaga, TMessage, T> factory,
            IPipe<SendContext<T>> pipe)
            where TSaga : class, ISaga
            where TMessage : class
        {
            if (!pipe.IsNotEmpty())
                return Create(factory);

            Task<SendTuple<T>> Factory(BehaviorContext<TSaga, TMessage> context)
            {
                var result = factory(context);
                return Task.FromResult(new SendTuple<T>(result, pipe));
            }

            return new ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(EventMessageFactory<TSaga, TMessage, T> factory,
            Action<SendContext<T>> callback)
            where TSaga : class, ISaga
            where TMessage : class
        {
            return callback == null ? Create(factory) : Create(factory, Pipe.Execute(callback));
        }

        public static ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> Create<TSaga, TMessage>(EventMessageFactory<TSaga, TMessage, T> factory,
            SendContextCallback<TSaga, TMessage, T> callback)
            where TSaga : class, ISaga
            where TMessage : class
        {
            if (callback == null)
                return Create(factory);

            Task<SendTuple<T>> Factory(BehaviorContext<TSaga, TMessage> context)
            {
                IPipe<SendContext<T>> callbackPipe = Pipe.Execute<SendContext<T>>(ctx => callback(context, ctx));

                var result = factory(context);
                return Task.FromResult(new SendTuple<T>(result, callbackPipe));
            }

            return new ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(T message,
            SendExceptionContextCallback<TSaga, TMessage, TException, T> callback)
            where TSaga : class, ISaga
            where TMessage : class
            where TException : Exception
        {
            return callback == null
                ? Create(message)
                : Create(context => message, callback);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(Task<T> factory,
            SendExceptionContextCallback<TSaga, TMessage, TException, T> callback)
            where TSaga : class, ISaga
            where TMessage : class
            where TException : Exception
        {
            return callback == null
                ? Create(factory)
                : Create(context => factory, callback);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(
            Func<BehaviorExceptionContext<TSaga, TMessage, TException>, Task<SendTuple<T>>> factory)
            where TSaga : class, ISaga
            where TMessage : class
            where TException : Exception
        {
            return new ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T>(factory);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(
            Func<BehaviorExceptionContext<TSaga, TMessage, TException>, Task<SendTuple<T>>> factory, Action<SendContext<T>> callback)
            where TSaga : class, ISaga
            where TMessage : class
            where TException : Exception
        {
            if (callback == null)
                return Create(factory);

            async Task<SendTuple<T>> Factory(BehaviorExceptionContext<TSaga, TMessage, TException> context)
            {
                SendTuple<T> result = await factory(context).ConfigureAwait(false);
                if (result.Pipe.IsNotEmpty())
                {
                    IPipe<SendContext<T>> pipe = result.Pipe.AddCallback(callback);
                    return new SendTuple<T>(result.Message, pipe);
                }

                return new SendTuple<T>(result.Message, Pipe.Execute(callback));
            }

            return new ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(
            Func<BehaviorExceptionContext<TSaga, TMessage, TException>, Task<SendTuple<T>>> factory,
            SendExceptionContextCallback<TSaga, TMessage, TException, T> callback)
            where TSaga : class, ISaga
            where TMessage : class
            where TException : Exception
        {
            if (callback == null)
                return Create(factory);

            async Task<SendTuple<T>> Factory(BehaviorExceptionContext<TSaga, TMessage, TException> context)
            {
                SendTuple<T> result = await factory(context).ConfigureAwait(false);
                if (result.Pipe.IsNotEmpty())
                {
                    IPipe<SendContext<T>> pipe = result.Pipe.AddCallback(ctx => callback(context, ctx));
                    return new SendTuple<T>(result.Message, pipe);
                }

                return new SendTuple<T>(result.Message, Pipe.Execute<SendContext<T>>(ctx => callback(context, ctx)));
            }

            return new ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(
            AsyncEventExceptionMessageFactory<TSaga, TMessage, TException, T> factory)
            where TSaga : class, ISaga
            where TMessage : class
            where TException : Exception
        {
            Task<SendTuple<T>> Factory(BehaviorExceptionContext<TSaga, TMessage, TException> context)
            {
                Task<T> result = factory(context);
                if (result.Status == TaskStatus.RanToCompletion)
                    return Task.FromResult(new SendTuple<T>(result.GetAwaiter().GetResult()));

                async Task<SendTuple<T>> GetResult()
                {
                    return new SendTuple<T>(await result.ConfigureAwait(false));
                }

                return GetResult();
            }

            return new ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(
            AsyncEventExceptionMessageFactory<TSaga, TMessage, TException, T> factory, IPipe<SendContext<T>> pipe)
            where TSaga : class, ISaga
            where TMessage : class
            where TException : Exception
        {
            if (!pipe.IsNotEmpty())
                return Create(factory);

            Task<SendTuple<T>> Factory(BehaviorExceptionContext<TSaga, TMessage, TException> context)
            {
                Task<T> result = factory(context);
                if (result.Status == TaskStatus.RanToCompletion)
                    return Task.FromResult(new SendTuple<T>(result.GetAwaiter().GetResult(), pipe));

                async Task<SendTuple<T>> GetResult()
                {
                    return new SendTuple<T>(await result.ConfigureAwait(false), pipe);
                }

                return GetResult();
            }

            return new ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(
            AsyncEventExceptionMessageFactory<TSaga, TMessage, TException, T> factory, Action<SendContext<T>> callback)
            where TSaga : class, ISaga
            where TMessage : class
            where TException : Exception
        {
            return callback == null ? Create(factory) : Create(factory, Pipe.Execute(callback));
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(
            AsyncEventExceptionMessageFactory<TSaga, TMessage, TException, T> factory, SendExceptionContextCallback<TSaga, TMessage, TException, T> callback)
            where TSaga : class, ISaga
            where TMessage : class
            where TException : Exception
        {
            if (callback == null)
                return Create(factory);

            Task<SendTuple<T>> Factory(BehaviorExceptionContext<TSaga, TMessage, TException> context)
            {
                IPipe<SendContext<T>> callbackPipe = Pipe.Execute<SendContext<T>>(ctx => callback(context, ctx));

                Task<T> result = factory(context);
                if (result.Status == TaskStatus.RanToCompletion)
                    return Task.FromResult(new SendTuple<T>(result.GetAwaiter().GetResult(), callbackPipe));

                async Task<SendTuple<T>> GetResult()
                {
                    return new SendTuple<T>(await result.ConfigureAwait(false), callbackPipe);
                }

                return GetResult();
            }

            return new ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(
            EventExceptionMessageFactory<TSaga, TMessage, TException, T> factory)
            where TSaga : class, ISaga
            where TMessage : class
            where TException : Exception
        {
            Task<SendTuple<T>> Factory(BehaviorExceptionContext<TSaga, TMessage, TException> context)
            {
                var result = factory(context);
                return Task.FromResult(new SendTuple<T>(result));
            }

            return new ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(
            EventExceptionMessageFactory<TSaga, TMessage, TException, T> factory, IPipe<SendContext<T>> pipe)
            where TSaga : class, ISaga
            where TMessage : class
            where TException : Exception
        {
            if (!pipe.IsNotEmpty())
                return Create(factory);

            Task<SendTuple<T>> Factory(BehaviorExceptionContext<TSaga, TMessage, TException> context)
            {
                var result = factory(context);
                return Task.FromResult(new SendTuple<T>(result, pipe));
            }

            return new ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(
            EventExceptionMessageFactory<TSaga, TMessage, TException, T> factory, Action<SendContext<T>> callback)
            where TSaga : class, ISaga
            where TMessage : class
            where TException : Exception
        {
            return callback == null ? Create(factory) : Create(factory, Pipe.Execute(callback));
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T> Create<TSaga, TMessage, TException>(
            EventExceptionMessageFactory<TSaga, TMessage, TException, T> factory, SendExceptionContextCallback<TSaga, TMessage, TException, T> callback)
            where TSaga : class, ISaga
            where TMessage : class
            where TException : Exception
        {
            if (callback == null)
                return Create(factory);

            Task<SendTuple<T>> Factory(BehaviorExceptionContext<TSaga, TMessage, TException> context)
            {
                IPipe<SendContext<T>> callbackPipe = Pipe.Execute<SendContext<T>>(ctx => callback(context, ctx));

                var result = factory(context);
                return Task.FromResult(new SendTuple<T>(result, callbackPipe));
            }

            return new ContextMessageFactory<BehaviorExceptionContext<TSaga, TMessage, TException>, T>(Factory);
        }

        // Saga Only

        public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(T message, SendContextCallback<TSaga, T> callback)
            where TSaga : class, ISaga
        {
            return callback == null
                ? Create(message)
                : Create(context => message, callback);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(Task<T> factory, SendContextCallback<TSaga, T> callback)
            where TSaga : class, ISaga
        {
            return callback == null
                ? Create(factory)
                : Create(context => factory, callback);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(T message,
            SendExceptionContextCallback<TSaga, TException, T> callback)
            where TSaga : class, ISaga
            where TException : Exception
        {
            return callback == null
                ? Create(message)
                : Create(context => message, callback);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(Task<T> factory,
            SendExceptionContextCallback<TSaga, TException, T> callback)
            where TSaga : class, ISaga
            where TException : Exception
        {
            return callback == null
                ? Create(factory)
                : Create(context => factory, callback);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(Func<BehaviorContext<TSaga>, Task<SendTuple<T>>> factory)
            where TSaga : class, ISaga
        {
            return new ContextMessageFactory<BehaviorContext<TSaga>, T>(factory);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(Func<BehaviorContext<TSaga>, Task<SendTuple<T>>> factory,
            Action<SendContext<T>> callback)
            where TSaga : class, ISaga
        {
            if (callback == null)
                return Create(factory);

            async Task<SendTuple<T>> Factory(BehaviorContext<TSaga> context)
            {
                SendTuple<T> result = await factory(context).ConfigureAwait(false);
                if (result.Pipe.IsNotEmpty())
                {
                    IPipe<SendContext<T>> pipe = result.Pipe.AddCallback(callback);
                    return new SendTuple<T>(result.Message, pipe);
                }

                return new SendTuple<T>(result.Message, Pipe.Execute(callback));
            }

            return new ContextMessageFactory<BehaviorContext<TSaga>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(Func<BehaviorContext<TSaga>, Task<SendTuple<T>>> factory,
            SendContextCallback<TSaga, T> callback)
            where TSaga : class, ISaga
        {
            if (callback == null)
                return Create(factory);

            async Task<SendTuple<T>> Factory(BehaviorContext<TSaga> context)
            {
                SendTuple<T> result = await factory(context).ConfigureAwait(false);
                if (result.Pipe.IsNotEmpty())
                {
                    IPipe<SendContext<T>> pipe = result.Pipe.AddCallback(ctx => callback(context, ctx));
                    return new SendTuple<T>(result.Message, pipe);
                }

                return new SendTuple<T>(result.Message, Pipe.Execute<SendContext<T>>(ctx => callback(context, ctx)));
            }

            return new ContextMessageFactory<BehaviorContext<TSaga>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(AsyncEventMessageFactory<TSaga, T> factory)
            where TSaga : class, ISaga
        {
            Task<SendTuple<T>> Factory(BehaviorContext<TSaga> context)
            {
                Task<T> result = factory(context);
                if (result.Status == TaskStatus.RanToCompletion)
                    return Task.FromResult(new SendTuple<T>(result.GetAwaiter().GetResult()));

                async Task<SendTuple<T>> GetResult()
                {
                    return new SendTuple<T>(await result.ConfigureAwait(false));
                }

                return GetResult();
            }

            return new ContextMessageFactory<BehaviorContext<TSaga>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(AsyncEventMessageFactory<TSaga, T> factory,
            IPipe<SendContext<T>> pipe)
            where TSaga : class, ISaga
        {
            if (!pipe.IsNotEmpty())
                return Create(factory);

            Task<SendTuple<T>> Factory(BehaviorContext<TSaga> context)
            {
                Task<T> result = factory(context);
                if (result.Status == TaskStatus.RanToCompletion)
                    return Task.FromResult(new SendTuple<T>(result.GetAwaiter().GetResult(), pipe));

                async Task<SendTuple<T>> GetResult()
                {
                    return new SendTuple<T>(await result.ConfigureAwait(false), pipe);
                }

                return GetResult();
            }

            return new ContextMessageFactory<BehaviorContext<TSaga>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(AsyncEventMessageFactory<TSaga, T> factory,
            Action<SendContext<T>> callback)
            where TSaga : class, ISaga
        {
            return callback == null ? Create(factory) : Create(factory, Pipe.Execute(callback));
        }

        public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(AsyncEventMessageFactory<TSaga, T> factory,
            SendContextCallback<TSaga, T> callback)
            where TSaga : class, ISaga
        {
            if (callback == null)
                return Create(factory);

            Task<SendTuple<T>> Factory(BehaviorContext<TSaga> context)
            {
                IPipe<SendContext<T>> callbackPipe = Pipe.Execute<SendContext<T>>(ctx => callback(context, ctx));

                Task<T> result = factory(context);
                if (result.Status == TaskStatus.RanToCompletion)
                    return Task.FromResult(new SendTuple<T>(result.GetAwaiter().GetResult(), callbackPipe));

                async Task<SendTuple<T>> GetResult()
                {
                    return new SendTuple<T>(await result.ConfigureAwait(false), callbackPipe);
                }

                return GetResult();
            }

            return new ContextMessageFactory<BehaviorContext<TSaga>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(EventMessageFactory<TSaga, T> factory)
            where TSaga : class, ISaga
        {
            Task<SendTuple<T>> Factory(BehaviorContext<TSaga> context)
            {
                var result = factory(context);
                return Task.FromResult(new SendTuple<T>(result));
            }

            return new ContextMessageFactory<BehaviorContext<TSaga>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(EventMessageFactory<TSaga, T> factory,
            IPipe<SendContext<T>> pipe)
            where TSaga : class, ISaga
        {
            if (!pipe.IsNotEmpty())
                return Create(factory);

            Task<SendTuple<T>> Factory(BehaviorContext<TSaga> context)
            {
                var result = factory(context);
                return Task.FromResult(new SendTuple<T>(result, pipe));
            }

            return new ContextMessageFactory<BehaviorContext<TSaga>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(EventMessageFactory<TSaga, T> factory,
            Action<SendContext<T>> callback)
            where TSaga : class, ISaga
        {
            return callback == null ? Create(factory) : Create(factory, Pipe.Execute(callback));
        }

        public static ContextMessageFactory<BehaviorContext<TSaga>, T> Create<TSaga>(EventMessageFactory<TSaga, T> factory,
            SendContextCallback<TSaga, T> callback)
            where TSaga : class, ISaga
        {
            if (callback == null)
                return Create(factory);

            Task<SendTuple<T>> Factory(BehaviorContext<TSaga> context)
            {
                IPipe<SendContext<T>> callbackPipe = Pipe.Execute<SendContext<T>>(ctx => callback(context, ctx));

                var result = factory(context);
                return Task.FromResult(new SendTuple<T>(result, callbackPipe));
            }

            return new ContextMessageFactory<BehaviorContext<TSaga>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(
            Func<BehaviorExceptionContext<TSaga, TException>, Task<SendTuple<T>>> factory)
            where TSaga : class, ISaga
            where TException : Exception
        {
            return new ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T>(factory);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(
            Func<BehaviorExceptionContext<TSaga, TException>, Task<SendTuple<T>>> factory, Action<SendContext<T>> callback)
            where TSaga : class, ISaga
            where TException : Exception
        {
            if (callback == null)
                return Create(factory);

            async Task<SendTuple<T>> Factory(BehaviorExceptionContext<TSaga, TException> context)
            {
                (var message, IPipe<SendContext<T>> sendPipe) = await factory(context).ConfigureAwait(false);
                if (sendPipe.IsNotEmpty())
                {
                    IPipe<SendContext<T>> pipe = sendPipe.AddCallback(callback);
                    return new SendTuple<T>(message, pipe);
                }

                return new SendTuple<T>(message, Pipe.Execute(callback));
            }

            return new ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(
            Func<BehaviorExceptionContext<TSaga, TException>, Task<SendTuple<T>>> factory, SendExceptionContextCallback<TSaga, TException, T> callback)
            where TSaga : class, ISaga
            where TException : Exception
        {
            if (callback == null)
                return Create(factory);

            async Task<SendTuple<T>> Factory(BehaviorExceptionContext<TSaga, TException> context)
            {
                (var message, IPipe<SendContext<T>> sendPipe) = await factory(context).ConfigureAwait(false);
                if (sendPipe.IsNotEmpty())
                {
                    IPipe<SendContext<T>> pipe = sendPipe.AddCallback(ctx => callback(context, ctx));
                    return new SendTuple<T>(message, pipe);
                }

                return new SendTuple<T>(message, Pipe.Execute<SendContext<T>>(ctx => callback(context, ctx)));
            }

            return new ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(
            AsyncEventExceptionMessageFactory<TSaga, TException, T> factory)
            where TSaga : class, ISaga
            where TException : Exception
        {
            Task<SendTuple<T>> Factory(BehaviorExceptionContext<TSaga, TException> context)
            {
                Task<T> result = factory(context);
                if (result.Status == TaskStatus.RanToCompletion)
                    return Task.FromResult(new SendTuple<T>(result.GetAwaiter().GetResult()));

                async Task<SendTuple<T>> GetResult()
                {
                    return new SendTuple<T>(await result.ConfigureAwait(false));
                }

                return GetResult();
            }

            return new ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(
            AsyncEventExceptionMessageFactory<TSaga, TException, T> factory, IPipe<SendContext<T>> pipe)
            where TSaga : class, ISaga
            where TException : Exception
        {
            if (!pipe.IsNotEmpty())
                return Create(factory);

            Task<SendTuple<T>> Factory(BehaviorExceptionContext<TSaga, TException> context)
            {
                Task<T> result = factory(context);
                if (result.Status == TaskStatus.RanToCompletion)
                    return Task.FromResult(new SendTuple<T>(result.GetAwaiter().GetResult(), pipe));

                async Task<SendTuple<T>> GetResult()
                {
                    return new SendTuple<T>(await result.ConfigureAwait(false), pipe);
                }

                return GetResult();
            }

            return new ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(
            AsyncEventExceptionMessageFactory<TSaga, TException, T> factory, Action<SendContext<T>> callback)
            where TSaga : class, ISaga
            where TException : Exception
        {
            return callback == null ? Create(factory) : Create(factory, Pipe.Execute(callback));
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(
            AsyncEventExceptionMessageFactory<TSaga, TException, T> factory, SendExceptionContextCallback<TSaga, TException, T> callback)
            where TSaga : class, ISaga
            where TException : Exception
        {
            if (callback == null)
                return Create(factory);

            Task<SendTuple<T>> Factory(BehaviorExceptionContext<TSaga, TException> context)
            {
                IPipe<SendContext<T>> callbackPipe = Pipe.Execute<SendContext<T>>(ctx => callback(context, ctx));

                Task<T> result = factory(context);
                if (result.Status == TaskStatus.RanToCompletion)
                    return Task.FromResult(new SendTuple<T>(result.GetAwaiter().GetResult(), callbackPipe));

                async Task<SendTuple<T>> GetResult()
                {
                    return new SendTuple<T>(await result.ConfigureAwait(false), callbackPipe);
                }

                return GetResult();
            }

            return new ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(
            EventExceptionMessageFactory<TSaga, TException, T> factory)
            where TSaga : class, ISaga
            where TException : Exception
        {
            Task<SendTuple<T>> Factory(BehaviorExceptionContext<TSaga, TException> context)
            {
                var result = factory(context);
                return Task.FromResult(new SendTuple<T>(result));
            }

            return new ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(
            EventExceptionMessageFactory<TSaga, TException, T> factory, IPipe<SendContext<T>> pipe)
            where TSaga : class, ISaga
            where TException : Exception
        {
            if (!pipe.IsNotEmpty())
                return Create(factory);

            Task<SendTuple<T>> Factory(BehaviorExceptionContext<TSaga, TException> context)
            {
                var result = factory(context);
                return Task.FromResult(new SendTuple<T>(result, pipe));
            }

            return new ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T>(Factory);
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(
            EventExceptionMessageFactory<TSaga, TException, T> factory, Action<SendContext<T>> callback)
            where TSaga : class, ISaga
            where TException : Exception
        {
            return callback == null ? Create(factory) : Create(factory, Pipe.Execute(callback));
        }

        public static ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T> Create<TSaga, TException>(
            EventExceptionMessageFactory<TSaga, TException, T> factory, SendExceptionContextCallback<TSaga, TException, T> callback)
            where TSaga : class, ISaga
            where TException : Exception
        {
            if (callback == null)
                return Create(factory);

            Task<SendTuple<T>> Factory(BehaviorExceptionContext<TSaga, TException> context)
            {
                IPipe<SendContext<T>> callbackPipe = Pipe.Execute<SendContext<T>>(ctx => callback(context, ctx));

                var result = factory(context);
                return Task.FromResult(new SendTuple<T>(result, callbackPipe));
            }

            return new ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, T>(Factory);
        }
    }
}

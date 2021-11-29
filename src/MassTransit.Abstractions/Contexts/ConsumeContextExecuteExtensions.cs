namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    public static class ConsumeContextExecuteExtensions
    {
        public static IPipe<ConsumeContext<T>> ToPipe<T>(this Action<ConsumeContext<T>> callback)
            where T : class
        {
            return new ConsumeContextPipe<T>(callback);
        }

        public static IPipe<ConsumeContext<T>> ToPipe<T>(this Func<ConsumeContext<T>, Task> callback)
            where T : class
        {
            return new ConsumeContextAsyncPipe<T>(callback);
        }

        public static IPipe<ConsumeContext> ToPipe(this Action<ConsumeContext> callback)
        {
            return new ConsumeContextPipe(callback);
        }

        public static IPipe<ConsumeContext> ToPipe(this Func<ConsumeContext, Task> callback)
        {
            return new ConsumeContextAsyncPipe(callback);
        }


        class ConsumeContextPipe<T> :
            IPipe<ConsumeContext<T>>
            where T : class
        {
            readonly Action<ConsumeContext<T>> _callback;

            public ConsumeContextPipe(Action<ConsumeContext<T>> callback)
            {
                _callback = callback;
            }

            public Task Send(ConsumeContext<T> context)
            {
                _callback(context);

                return Task.CompletedTask;
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("sendContextCallback");
            }
        }


        class ConsumeContextAsyncPipe<T> :
            IPipe<ConsumeContext<T>>
            where T : class
        {
            readonly Func<ConsumeContext<T>, Task> _callback;

            public ConsumeContextAsyncPipe(Func<ConsumeContext<T>, Task> callback)
            {
                _callback = callback;
            }

            public Task Send(ConsumeContext<T> context)
            {
                return _callback(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("sendContextCallback");
            }
        }


        class ConsumeContextPipe :
            IPipe<ConsumeContext>
        {
            readonly Action<ConsumeContext> _callback;

            public ConsumeContextPipe(Action<ConsumeContext> callback)
            {
                _callback = callback;
            }

            public Task Send(ConsumeContext context)
            {
                _callback(context);

                return Task.CompletedTask;
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("sendContextCallback");
            }
        }


        class ConsumeContextAsyncPipe :
            IPipe<ConsumeContext>
        {
            readonly Func<ConsumeContext, Task> _callback;

            public ConsumeContextAsyncPipe(Func<ConsumeContext, Task> callback)
            {
                _callback = callback;
            }

            public Task Send(ConsumeContext context)
            {
                return _callback(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("sendContextCallback");
            }
        }
    }
}

namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;


    public class RequestHandlerConsumer<TMessage, TResponse> :
        IConsumer<TMessage>
        where TMessage : class
        where TResponse : class
    {
        readonly Func<ConsumeContext<TMessage>, Task<TResponse>> _handler;

        public RequestHandlerConsumer(RequestHandlerMethod<TMessage, TResponse> method)
        {
            _handler = method.Handler;
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var response = await _handler(context).ConfigureAwait(false);

            if (response != null)
                await context.RespondAsync(response).ConfigureAwait(false);
        }
    }


    public class RequestHandlerConsumer<TMessage, T1, TResponse> :
        IConsumer<TMessage>
        where TMessage : class
        where TResponse : class
        where T1 : class
    {
        readonly T1 _arg1;
        readonly Func<ConsumeContext<TMessage>, T1, Task<TResponse>> _handler;

        public RequestHandlerConsumer(RequestHandlerMethod<TMessage, T1, TResponse> method, T1 arg1)
        {
            _arg1 = arg1;
            _handler = method.Handler;
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var response = await _handler(context, _arg1).ConfigureAwait(false);

            if (response != null)
                await context.RespondAsync(response).ConfigureAwait(false);
        }
    }


    public class RequestHandlerConsumer<TMessage, T1, T2, TResponse> :
        IConsumer<TMessage>
        where TMessage : class
        where TResponse : class
        where T1 : class
        where T2 : class
    {
        readonly T1 _arg1;
        readonly T2 _arg2;
        readonly Func<ConsumeContext<TMessage>, T1, T2, Task<TResponse>> _handler;

        public RequestHandlerConsumer(RequestHandlerMethod<TMessage, T1, T2, TResponse> method, T1 arg1, T2 arg2)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _handler = method.Handler;
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var response = await _handler(context, _arg1, _arg2).ConfigureAwait(false);

            if (response != null)
                await context.RespondAsync(response).ConfigureAwait(false);
        }
    }


    public class RequestHandlerConsumer<TMessage, T1, T2, T3, TResponse> :
        IConsumer<TMessage>
        where TMessage : class
        where TResponse : class
        where T1 : class
        where T2 : class
        where T3 : class
    {
        readonly T1 _arg1;
        readonly T2 _arg2;
        readonly T3 _arg3;
        readonly Func<ConsumeContext<TMessage>, T1, T2, T3, Task<TResponse>> _handler;

        public RequestHandlerConsumer(RequestHandlerMethod<TMessage, T1, T2, T3, TResponse> method, T1 arg1, T2 arg2, T3 arg3)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _handler = method.Handler;
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var response = await _handler(context, _arg1, _arg2, _arg3).ConfigureAwait(false);

            if (response != null)
                await context.RespondAsync(response).ConfigureAwait(false);
        }
    }
}

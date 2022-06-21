namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;


    public class RequestHandlerMethod<TMessage, TResponse>
        where TMessage : class
        where TResponse : class
    {
        public RequestHandlerMethod(Func<ConsumeContext<TMessage>, Task<TResponse>> handler)
        {
            Handler = handler;
        }

        public RequestHandlerMethod(Func<TMessage, Task<TResponse>> handler)
        {
            Handler = context => handler(context.Message);
        }

        public Func<ConsumeContext<TMessage>, Task<TResponse>> Handler { get; }
    }


    public class RequestHandlerMethod<TMessage, T1, TResponse>
        where TMessage : class
        where T1 : class
        where TResponse : class
    {
        public RequestHandlerMethod(Func<ConsumeContext<TMessage>, T1, Task<TResponse>> handler)
        {
            Handler = handler;
        }

        public RequestHandlerMethod(Func<TMessage, T1, Task<TResponse>> handler)
        {
            Handler = (context, arg1) => handler(context.Message, arg1);
        }

        public Func<ConsumeContext<TMessage>, T1, Task<TResponse>> Handler { get; }
    }


    public class RequestHandlerMethod<TMessage, T1, T2, TResponse>
        where TMessage : class
        where T1 : class
        where T2 : class
        where TResponse : class
    {
        public RequestHandlerMethod(Func<ConsumeContext<TMessage>, T1, T2, Task<TResponse>> handler)
        {
            Handler = handler;
        }

        public RequestHandlerMethod(Func<TMessage, T1, T2, Task<TResponse>> handler)
        {
            Handler = (context, arg1, arg2) => handler(context.Message, arg1, arg2);
        }

        public Func<ConsumeContext<TMessage>, T1, T2, Task<TResponse>> Handler { get; }
    }


    public class RequestHandlerMethod<TMessage, T1, T2, T3, TResponse>
        where TMessage : class
        where T1 : class
        where T2 : class
        where T3 : class
        where TResponse : class
    {
        public RequestHandlerMethod(Func<ConsumeContext<TMessage>, T1, T2, T3, Task<TResponse>> handler)
        {
            Handler = handler;
        }

        public RequestHandlerMethod(Func<TMessage, T1, T2, T3, Task<TResponse>> handler)
        {
            Handler = (context, arg1, arg2, arg3) => handler(context.Message, arg1, arg2, arg3);
        }

        public Func<ConsumeContext<TMessage>, T1, T2, T3, Task<TResponse>> Handler { get; }
    }
}

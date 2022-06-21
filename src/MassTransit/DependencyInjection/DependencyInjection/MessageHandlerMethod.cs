namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;


    public class MessageHandlerMethod<TMessage>
        where TMessage : class
    {
        public MessageHandlerMethod(Func<ConsumeContext<TMessage>, Task> handler)
        {
            Handler = handler;
        }

        public MessageHandlerMethod(Func<TMessage, Task> handler)
        {
            Handler = context => handler(context.Message);
        }

        public Func<ConsumeContext<TMessage>, Task> Handler { get; }
    }


    public class MessageHandlerMethod<TMessage, T1>
        where TMessage : class
        where T1 : class
    {
        public MessageHandlerMethod(Func<ConsumeContext<TMessage>, T1, Task> handler)
        {
            Handler = handler;
        }

        public MessageHandlerMethod(Func<TMessage, T1, Task> handler)
        {
            Handler = (context, arg1) => handler(context.Message, arg1);
        }

        public Func<ConsumeContext<TMessage>, T1, Task> Handler { get; }
    }


    public class MessageHandlerMethod<TMessage, T1, T2>
        where TMessage : class
        where T1 : class
        where T2 : class
    {
        public MessageHandlerMethod(Func<ConsumeContext<TMessage>, T1, T2, Task> handler)
        {
            Handler = handler;
        }

        public MessageHandlerMethod(Func<TMessage, T1, T2, Task> handler)
        {
            Handler = (context, arg1, arg2) => handler(context.Message, arg1, arg2);
        }

        public Func<ConsumeContext<TMessage>, T1, T2, Task> Handler { get; }
    }


    public class MessageHandlerMethod<TMessage, T1, T2, T3>
        where TMessage : class
        where T1 : class
        where T2 : class
    {
        public MessageHandlerMethod(Func<ConsumeContext<TMessage>, T1, T2, T3, Task> handler)
        {
            Handler = handler;
        }

        public MessageHandlerMethod(Func<TMessage, T1, T2, T3, Task> handler)
        {
            Handler = (context, arg1, arg2, arg3) => handler(context.Message, arg1, arg2, arg3);
        }

        public Func<ConsumeContext<TMessage>, T1, T2, T3, Task> Handler { get; }
    }
}

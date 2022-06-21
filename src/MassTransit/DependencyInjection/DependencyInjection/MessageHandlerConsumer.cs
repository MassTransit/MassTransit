namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;


    public class MessageHandlerConsumer<T> :
        IConsumer<T>
        where T : class
    {
        readonly Func<ConsumeContext<T>, Task> _handler;

        public MessageHandlerConsumer(MessageHandlerMethod<T> method)
        {
            _handler = method.Handler;
        }

        public Task Consume(ConsumeContext<T> context)
        {
            return _handler(context);
        }
    }


    public class MessageHandlerConsumer<T, T1> :
        IConsumer<T>
        where T : class
        where T1 : class
    {
        readonly T1 _arg1;
        readonly Func<ConsumeContext<T>, T1, Task> _handler;

        public MessageHandlerConsumer(MessageHandlerMethod<T, T1> method, T1 arg1)
        {
            _handler = method.Handler;

            _arg1 = arg1;
        }

        public Task Consume(ConsumeContext<T> context)
        {
            return _handler(context, _arg1);
        }
    }


    public class MessageHandlerConsumer<T, T1, T2> :
        IConsumer<T>
        where T : class
        where T1 : class
        where T2 : class
    {
        readonly T1 _arg1;
        readonly T2 _arg2;
        readonly Func<ConsumeContext<T>, T1, T2, Task> _handler;

        public MessageHandlerConsumer(MessageHandlerMethod<T, T1, T2> method, T1 arg1, T2 arg2)
        {
            _handler = method.Handler;

            _arg1 = arg1;
            _arg2 = arg2;
        }

        public Task Consume(ConsumeContext<T> context)
        {
            return _handler(context, _arg1, _arg2);
        }
    }


    public class MessageHandlerConsumer<T, T1, T2, T3> :
        IConsumer<T>
        where T : class
        where T1 : class
        where T2 : class
        where T3 : class
    {
        readonly T1 _arg1;
        readonly T2 _arg2;
        readonly T3 _arg3;
        readonly Func<ConsumeContext<T>, T1, T2, T3, Task> _handler;

        public MessageHandlerConsumer(MessageHandlerMethod<T, T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3)
        {
            _handler = method.Handler;

            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        public Task Consume(ConsumeContext<T> context)
        {
            return _handler(context, _arg1, _arg2, _arg3);
        }
    }
}

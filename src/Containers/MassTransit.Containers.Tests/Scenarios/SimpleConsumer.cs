namespace MassTransit.Containers.Tests.Scenarios
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class SimpleConsumer :
        IConsumer<SimpleMessageInterface>
    {
        static readonly TaskCompletionSource<SimpleConsumer> _consumerCreated = TaskUtil.GetTask<SimpleConsumer>();

        readonly ISimpleConsumerDependency _dependency;
        readonly TaskCompletionSource<SimpleMessageInterface> _received;

        public SimpleConsumer(ISimpleConsumerDependency dependency)
        {
            _dependency = dependency;
            Console.WriteLine("SimpleConsumer()");

            _received = TaskUtil.GetTask<SimpleMessageInterface>();

            _consumerCreated.TrySetResult(this);
        }

        public Task<SimpleMessageInterface> Last
        {
            get { return _received.Task; }
        }

        public ISimpleConsumerDependency Dependency
        {
            get { return _dependency; }
        }

        public static Task<SimpleConsumer> LastConsumer
        {
            get { return _consumerCreated.Task; }
        }

        public async Task Consume(ConsumeContext<SimpleMessageInterface> message)
        {
            _dependency.DoSomething();

            _received.TrySetResult(message.Message);
        }
    }


    public class SimplerConsumer :
        IConsumer<SimpleMessageInterface>
    {
        static readonly TaskCompletionSource<SimplerConsumer> _consumerCreated = TaskUtil.GetTask<SimplerConsumer>();

        readonly TaskCompletionSource<SimpleMessageInterface> _received;

        public SimplerConsumer()
        {
            _received = TaskUtil.GetTask<SimpleMessageInterface>();

            _consumerCreated.TrySetResult(this);
        }

        public Task<SimpleMessageInterface> Last
        {
            get { return _received.Task; }
        }

        public static Task<SimplerConsumer> LastConsumer
        {
            get { return _consumerCreated.Task; }
        }

        public async Task Consume(ConsumeContext<SimpleMessageInterface> message)
        {
            _received.TrySetResult(message.Message);
        }
    }
}

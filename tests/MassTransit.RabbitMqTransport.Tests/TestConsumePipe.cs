namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline;
    using Util;


    class TestConsumePipe :
        IConsumePipe
    {
        readonly Func<ConsumeContext, Task> _callback;

        public TestConsumePipe(Func<ConsumeContext, Task> callback)
        {
            _callback = callback;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        public async Task Send(ConsumeContext context)
        {
            await Task.Yield();

            await _callback(context);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            throw new NotImplementedException();
        }

        public ConnectHandle ConnectConsumeMessageObserver<TMessage>(IConsumeMessageObserver<TMessage> observer)
            where TMessage : class
        {
            throw new NotImplementedException();
        }

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            throw new NotImplementedException();
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            throw new NotImplementedException();
        }

        public Task Connected => TaskUtil.Completed;
    }
}

namespace MassTransit.Containers.Tests.Scenarios
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Saga;
    using Util;


    public class SimpleSaga :
        ISaga,
        InitiatedBy<FirstSagaMessage>,
        Orchestrates<SecondSagaMessage>,
        Observes<ThirdSagaMessage, SimpleSaga>
    {
        readonly TaskCompletionSource<FirstSagaMessage> _first = TaskUtil.GetTask<FirstSagaMessage>();
        readonly TaskCompletionSource<SecondSagaMessage> _second = TaskUtil.GetTask<SecondSagaMessage>();
        readonly TaskCompletionSource<ThirdSagaMessage> _third = TaskUtil.GetTask<ThirdSagaMessage>();

        public SimpleSaga(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        protected SimpleSaga()
        {
        }

        public Task<FirstSagaMessage> First
        {
            get { return _first.Task; }
        }

        public Task<SecondSagaMessage> Second
        {
            get { return _second.Task; }
        }

        public Task<ThirdSagaMessage> Third
        {
            get { return _third.Task; }
        }

        public async Task Consume(ConsumeContext<FirstSagaMessage> context)
        {
            Console.WriteLine("SimpleSaga: First: {0}", context.Message.CorrelationId);
            _first.TrySetResult(context.Message);
        }

        public Guid CorrelationId { get; set; }

        public async Task Consume(ConsumeContext<ThirdSagaMessage> context)
        {
            Console.WriteLine("SimpleSaga: Third: {0}", context.Message.CorrelationId);
            _third.TrySetResult(context.Message);
        }

        Expression<Func<SimpleSaga, ThirdSagaMessage, bool>> Observes<ThirdSagaMessage, SimpleSaga>.CorrelationExpression
        {
            get { return (saga, message) => saga.CorrelationId == message.CorrelationId; }
        }

        public async Task Consume(ConsumeContext<SecondSagaMessage> context)
        {
            Console.WriteLine("SimpleSaga: Second: {0}", context.Message.CorrelationId);
            _second.TrySetResult(context.Message);
        }
    }
}

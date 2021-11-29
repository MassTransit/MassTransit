namespace MassTransit.Containers.Tests.Scenarios
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class SecondSimpleSaga :
        ISaga,
        InitiatedBy<FirstSagaMessage>
    {
        readonly TaskCompletionSource<FirstSagaMessage> _first = TaskUtil.GetTask<FirstSagaMessage>();

        public SecondSimpleSaga(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        protected SecondSimpleSaga()
        {
        }

        public Task<FirstSagaMessage> First => _first.Task;

        public async Task Consume(ConsumeContext<FirstSagaMessage> context)
        {
            Console.WriteLine("SimpleSaga: First: {0}", context.Message.CorrelationId);
            _first.TrySetResult(context.Message);
        }

        public Guid CorrelationId { get; set; }
    }


    public class SecondSimpleSagaDefinition :
        SagaDefinition<SecondSimpleSaga>
    {
        public SecondSimpleSagaDefinition()
        {
            EndpointName = "custom-second-saga";
        }
    }
}

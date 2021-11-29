namespace MassTransit.MartenIntegration.Tests
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Marten.Schema;


    public class SimpleSaga :
        InitiatedBy<InitiateSimpleSaga>,
        Observes<ObservableSagaMessage, SimpleSaga>,
        Orchestrates<CompleteSimpleSaga>,
        ISaga
    {
        public bool Completed { get; private set; }
        public bool Initiated { get; private set; }
        public bool Observed { get; private set; }
        public string Name { get; private set; }

        public async Task Consume(ConsumeContext<InitiateSimpleSaga> context)
        {
            Initiated = true;
            Name = context.Message.Name;
        }

        [Identity]
        public Guid CorrelationId { get; set; }

        public async Task Consume(ConsumeContext<ObservableSagaMessage> message)
        {
            Observed = true;
        }

        Expression<Func<SimpleSaga, ObservableSagaMessage, bool>> Observes<ObservableSagaMessage, SimpleSaga>.CorrelationExpression
        {
            get { return (saga, message) => saga.Name == message.Name; }
        }

        public async Task Consume(ConsumeContext<CompleteSimpleSaga> message)
        {
            Completed = true;
        }
    }
}

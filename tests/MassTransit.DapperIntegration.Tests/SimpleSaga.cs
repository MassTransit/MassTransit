namespace MassTransit.DapperIntegration.Tests
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Dapper.Contrib.Extensions;
    using MassTransit.Tests.Saga.Messages;


    public class SimpleSaga :
        InitiatedBy<InitiateSimpleSaga>,
        Orchestrates<CompleteSimpleSaga>,
        Observes<ObservableSagaMessage, SimpleSaga>,
        ISaga
    {
        public bool Completed { get; private set; }
        public bool Initiated { get; private set; }
        public bool Observed { get; private set; }
        public string Name { get; private set; }

        public string CorrelateBySomething { get; set; }

        public async Task Consume(ConsumeContext<InitiateSimpleSaga> context)
        {
            Initiated = true;
            Name = context.Message.Name;
        }

        [ExplicitKey]
        public Guid CorrelationId { get; set; }

        public async Task Consume(ConsumeContext<ObservableSagaMessage> message)
        {
            Observed = true;
        }

        [Computed]
        public Expression<Func<SimpleSaga, ObservableSagaMessage, bool>> CorrelationExpression
        {
            get { return (saga, message) => saga.Name == message.Name; }
        }

        public async Task Consume(ConsumeContext<CompleteSimpleSaga> message)
        {
            Completed = true;
        }
    }
}

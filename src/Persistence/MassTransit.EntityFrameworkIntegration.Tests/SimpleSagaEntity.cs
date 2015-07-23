namespace MassTransit.EntityFrameworkIntegration.Tests
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using MassTransit.Tests.Saga.Messages;

    /// <summary>
    /// Nearly a complete copy from MassTransit.Tests.Saga.SimpleSaga, but had to use SimpleSagaEntity with the setter available
    /// </summary>
    public class SimpleSagaEntity :
        ISagaEntity,
        InitiatedBy<InitiateSimpleSaga>,
        Orchestrates<CompleteSimpleSaga>,
        Observes<ObservableSagaMessage, SimpleSagaEntity>,
        ISaga
    {
        public SimpleSagaEntity()
        {
        }

        public SimpleSagaEntity(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public bool Completed { get; private set; }
        public bool Initiated { get; private set; }
        public bool Observed { get; private set; }
        public string Name { get; private set; }

        public async Task Consume(ConsumeContext<InitiateSimpleSaga> context)
        {
            Initiated = true;
            Name = context.Message.Name;
        }

        public  Guid CorrelationId { get; set; }

        public async Task Consume(ConsumeContext<ObservableSagaMessage> message)
        {
            Observed = true;
        }

        public Expression<Func<SimpleSagaEntity, ObservableSagaMessage, bool>> CorrelationExpression
        {
            get { return (saga, message) => saga.Name == message.Name; }
        }

        public async Task Consume(ConsumeContext<CompleteSimpleSaga> message)
        {
            Completed = true;
        }
    }
}

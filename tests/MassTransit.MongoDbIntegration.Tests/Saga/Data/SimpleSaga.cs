namespace MassTransit.MongoDbIntegration.Tests.Saga.Data
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using MongoDB.Bson.Serialization.Attributes;


    public class SimpleSaga :
        InitiatedBy<InitiateSimpleSaga>,
        Orchestrates<CompleteSimpleSaga>,
        Observes<ObservableSagaMessage, SimpleSaga>,
        ISagaVersion
    {
        public bool Completed { get; private set; }

        public bool Initiated { get; private set; }

        public bool Observed { get; private set; }

        public string Name { get; private set; }

        public Task Consume(ConsumeContext<InitiateSimpleSaga> context)
        {
            Initiated = true;
            Name = context.Message.Name;

            return Task.FromResult(0);
        }

        [BsonId]
        public Guid CorrelationId { get; set; }

        public int Version { get; set; }

        public Task Consume(ConsumeContext<ObservableSagaMessage> message)
        {
            Observed = true;

            return Task.FromResult(0);
        }

        public Expression<Func<SimpleSaga, ObservableSagaMessage, bool>> CorrelationExpression
        {
            get { return (saga, message) => saga.Name == message.Name; }
        }

        public Task Consume(ConsumeContext<CompleteSimpleSaga> message)
        {
            Completed = true;

            return Task.FromResult(0);
        }
    }
}

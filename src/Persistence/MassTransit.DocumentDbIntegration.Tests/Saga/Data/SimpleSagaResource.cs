namespace MassTransit.DocumentDbIntegration.Tests.Saga.Data
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Saga;
    using Messages;
    using Microsoft.Azure.Documents;
    using Newtonsoft.Json;


    public class SimpleSagaResource :
        Resource,
        InitiatedBy<InitiateSimpleSaga>,
        Orchestrates<CompleteSimpleSaga>,
        Observes<ObservableSagaMessage, SimpleSaga>,
        IVersionedSaga
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

        [JsonProperty("id")]
        public Guid CorrelationId { get; set; }

        public Task Consume(ConsumeContext<ObservableSagaMessage> message)
        {
            Observed = true;

            return Task.FromResult(0);
        }

        [JsonIgnore]
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

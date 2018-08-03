namespace MassTransit.DocumentDbIntegration.Saga.Context
{
    using Microsoft.Azure.Documents;
    using Newtonsoft.Json;

    public interface IDocumentDbSagaConsumeContextFactory
    {
        SagaConsumeContext<TSaga, TMessage> Create<TSaga, TMessage>(IDocumentClient client, string databaseName, string collectionName,
            ConsumeContext<TMessage> message, TSaga instance, bool existing = true, JsonSerializerSettings jsonSerializerSettings = null)
            where TSaga : class, IVersionedSaga
            where TMessage : class;
    }
}
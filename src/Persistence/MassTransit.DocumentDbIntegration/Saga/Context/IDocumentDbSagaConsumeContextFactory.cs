namespace MassTransit.DocumentDbIntegration.Saga.Context
{
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;


    public interface IDocumentDbSagaConsumeContextFactory
    {
        SagaConsumeContext<TSaga, TMessage> Create<TSaga, TMessage>(IDocumentClient client, string databaseName, string collectionName,
            ConsumeContext<TMessage> message, TSaga instance, bool existing = true, RequestOptions requestOptions = null)
            where TSaga : class, IVersionedSaga
            where TMessage : class;
    }
}

namespace MassTransit.DocumentDbIntegration.Saga.Context
{
    using MassTransit.Saga;
    using Microsoft.Azure.Documents;

    public interface IDocumentDbSagaConsumeContextFactory
    {
        SagaConsumeContext<TSaga, TMessage> Create<TSaga, TMessage>(IDocumentClient client, string databaseName, string collectionName, ConsumeContext<TMessage> message, TSaga instance, bool existing = true) 
            where TSaga : class, ISaga 
            where TMessage : class;
    }
}
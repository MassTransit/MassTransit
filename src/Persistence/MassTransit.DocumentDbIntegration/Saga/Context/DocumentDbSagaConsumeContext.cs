namespace MassTransit.DocumentDbIntegration.Saga.Context
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Context;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Util;


    public class DocumentDbSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextProxyScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, IVersionedSaga
    {
        readonly IDocumentClient _client;
        readonly string _collectionName;
        readonly string _databaseName;
        readonly bool _existing;
        readonly RequestOptions _requestOptions;

        public DocumentDbSagaConsumeContext(IDocumentClient client, string databaseName, string collectionName, ConsumeContext<TMessage> context,
            TSaga instance, bool existing = true, RequestOptions requestOptions = null)
            : base(context)
        {
            Saga = instance;
            _client = client;
            _existing = existing;
            _databaseName = databaseName;
            _collectionName = collectionName;
            _requestOptions = requestOptions ?? new RequestOptions();
        }

        Guid? MessageContext.CorrelationId => Saga.CorrelationId;

        public async Task SetCompleted()
        {
            IsCompleted = true;

            if (_existing)
            {
                await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, Saga.CorrelationId.ToString()), _requestOptions)
                    .ConfigureAwait(false);

                LogContext.Debug?.Log("SAGA:{SagaType}:{CorrelationId} Removed {MessageType}", TypeMetadataCache<TSaga>.ShortName, Saga.CorrelationId,
                    TypeMetadataCache<TMessage>.ShortName);
            }
        }

        public TSaga Saga { get; }

        public bool IsCompleted { get; private set; }
    }
}

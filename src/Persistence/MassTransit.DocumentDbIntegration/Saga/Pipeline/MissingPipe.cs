namespace MassTransit.DocumentDbIntegration.Saga.Pipeline
{
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using MassTransit.Saga;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Newtonsoft.Json;


    public class MissingPipe<TSaga, TMessage> :
        IPipe<SagaConsumeContext<TSaga, TMessage>>
        where TSaga : class, IVersionedSaga
        where TMessage : class
    {
        readonly IDocumentClient _client;
        readonly string _collectionName;
        readonly string _databaseName;
        readonly IDocumentDbSagaConsumeContextFactory _documentDbSagaConsumeContextFactory;
        readonly JsonSerializerSettings _jsonSerializerSettings;
        readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _next;
        readonly RequestOptions _requestOptions;

        public MissingPipe(IDocumentClient client, string databaseName, string collectionName, IPipe<SagaConsumeContext<TSaga, TMessage>> next,
            IDocumentDbSagaConsumeContextFactory documentDbSagaConsumeContextFactory, JsonSerializerSettings jsonSerializerSettings)
        {
            _client = client;
            _databaseName = databaseName;
            _collectionName = collectionName;
            _next = next;
            _documentDbSagaConsumeContextFactory = documentDbSagaConsumeContextFactory;
            _jsonSerializerSettings = jsonSerializerSettings;
            if (_jsonSerializerSettings != null)
                _requestOptions = new RequestOptions {JsonSerializerSettings = jsonSerializerSettings};
        }

        public void Probe(ProbeContext context)
        {
            _next.Probe(context);
        }

        public async Task Send(SagaConsumeContext<TSaga, TMessage> context)
        {
            SagaConsumeContext<TSaga, TMessage> proxy =
                _documentDbSagaConsumeContextFactory.Create(_client, _databaseName, _collectionName, context, context.Saga, false, _requestOptions);

            proxy.LogAdded();

            await _next.Send(proxy).ConfigureAwait(false);

            if (!proxy.IsCompleted)
                await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), context.Saga, _requestOptions, true)
                    .ConfigureAwait(false);
        }
    }
}

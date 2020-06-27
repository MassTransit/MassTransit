namespace MassTransit.Azure.Table.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;
    using Saga;


    public class AzureTableDatabaseContext<TSaga> :
        DatabaseContext<TSaga>
        where TSaga : class, IVersionedSaga
    {
        readonly string _partitionKey;
        readonly CloudTable _table;

        public AzureTableDatabaseContext(CloudTable table)
        {
            _table = table;
            _partitionKey = typeof(TSaga).Name;
        }

        public Task Add(SagaConsumeContext<TSaga> context)
        {
            return Insert(context.Saga, context.CancellationToken);
        }

        public async Task Insert(TSaga instance, CancellationToken cancellationToken)
        {
            IDictionary<string, EntityProperty> entityProperties = TableEntity.Flatten(instance, new OperationContext());

            var operation = TableOperation.Insert(new DynamicTableEntity(_partitionKey, instance.CorrelationId.ToString()) {Properties = entityProperties});
            var result = await _table.ExecuteAsync(operation, cancellationToken).ConfigureAwait(false);

            instance.ETag = result.Etag;
        }

        public async Task<TSaga> Load(Guid correlationId, CancellationToken cancellationToken)
        {
            var operation = TableOperation.Retrieve<DynamicTableEntity>(_partitionKey, correlationId.ToString());
            var result = await _table.ExecuteAsync(operation, cancellationToken).ConfigureAwait(false);

            if (result.Result is DynamicTableEntity tableEntity)
            {
                var instance = TableEntity.ConvertBack<TSaga>(tableEntity.Properties, new OperationContext());

                instance.ETag = tableEntity.ETag;

                return instance;
            }

            return default;
        }

        public async Task Update(SagaConsumeContext<TSaga> context)
        {
            var instance = context.Saga;

            try
            {
                IDictionary<string, EntityProperty> entityProperties = TableEntity.Flatten(instance, new OperationContext());
                var operation = TableOperation.Replace(new DynamicTableEntity(_partitionKey, instance.CorrelationId.ToString())
                {
                    Properties = entityProperties,
                    ETag = instance.ETag
                });
                await _table.ExecuteAsync(operation, context.CancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new SagaException("Saga update failed", typeof(TSaga), instance.CorrelationId, exception);
            }
        }

        public async Task Delete(SagaConsumeContext<TSaga> context)
        {
            var instance = context.Saga;

            IDictionary<string, EntityProperty> entityProperties = TableEntity.Flatten(instance, new OperationContext());

            var operation = TableOperation.Delete(new DynamicTableEntity(_partitionKey, instance.CorrelationId.ToString(), instance.ETag, entityProperties));

            await _table.ExecuteAsync(operation, context.CancellationToken).ConfigureAwait(false);
        }
    }
}

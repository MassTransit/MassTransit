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
        readonly ISagaKeyFormatter<TSaga> _keyFormatter;
        readonly CloudTable _table;

        public AzureTableDatabaseContext(CloudTable table, ISagaKeyFormatter<TSaga> keyFormatter)
        {
            _table = table;
            _keyFormatter = keyFormatter;
        }

        public Task Add(SagaConsumeContext<TSaga> context)
        {
            return Insert(context.Saga, context.CancellationToken);
        }

        public async Task Insert(TSaga instance, CancellationToken cancellationToken)
        {
            IDictionary<string, EntityProperty> entityProperties = TableEntity.Flatten(instance, new OperationContext());
            entityProperties.Remove(nameof(IVersionedSaga.ETag));

            var (partitionKey, rowKey) = _keyFormatter.Format(instance.CorrelationId);

            var operation = TableOperation.Insert(new DynamicTableEntity(partitionKey, rowKey) {Properties = entityProperties});
            var result = await _table.ExecuteAsync(operation, cancellationToken).ConfigureAwait(false);
            if (result.Result is DynamicTableEntity tableEntity)
                instance.ETag = tableEntity.ETag;
        }

        public async Task<TSaga> Load(Guid correlationId, CancellationToken cancellationToken)
        {
            var (partitionKey, rowKey) = _keyFormatter.Format(correlationId);

            var operation = TableOperation.Retrieve<DynamicTableEntity>(partitionKey, rowKey);
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
                entityProperties.Remove(nameof(IVersionedSaga.ETag));

                var (partitionKey, rowKey) = _keyFormatter.Format(instance.CorrelationId);

                var operation = TableOperation.Replace(new DynamicTableEntity(partitionKey, rowKey)
                {
                    Properties = entityProperties,
                    ETag = instance.ETag
                });
                var result = await _table.ExecuteAsync(operation, context.CancellationToken).ConfigureAwait(false);
                if (result.Result is DynamicTableEntity tableEntity)
                    instance.ETag = tableEntity.ETag;
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

            var (partitionKey, rowKey) = _keyFormatter.Format(instance.CorrelationId);

            var operation = TableOperation.Delete(new DynamicTableEntity(partitionKey, rowKey, instance.ETag, entityProperties));

            await _table.ExecuteAsync(operation, context.CancellationToken).ConfigureAwait(false);
        }
    }
}

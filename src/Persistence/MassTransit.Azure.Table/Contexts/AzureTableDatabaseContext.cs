namespace MassTransit.Azure.Table
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;


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
            return Insert(context.Saga);
        }

        public Task Insert(TSaga instance)
        {
            Guid? rowKey = instance.CorrelationId;
            IDictionary<string, EntityProperty> dynamicEntity = TableEntity.Flatten(instance, new OperationContext());
            return _table.ExecuteAsync(TableOperation.InsertOrReplace(new DynamicTableEntity(_partitionKey, rowKey.ToString()) {Properties = dynamicEntity}),
                CancellationToken.None);
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return Get(correlationId);
        }

        public async Task Update(SagaConsumeContext<TSaga> context)
        {
            var instance = context.Saga;

            try
            {
                var existingInstanceTableEntity = await GetTableEntity(instance.CorrelationId).ConfigureAwait(false);
                var existingInstance = ConvertTableEntityToSagaInstance(existingInstanceTableEntity);
                existingInstance.ETag = existingInstanceTableEntity.ETag;

                Guid? rowKey = instance.CorrelationId;
                IDictionary<string, EntityProperty> dynamicEntity = TableEntity.Flatten(instance, new OperationContext());
                var replacementOperation = TableOperation.Replace(new DynamicTableEntity(_partitionKey, rowKey.ToString())
                {
                    Properties = dynamicEntity,
                    ETag = existingInstance.ETag
                });
                await _table.ExecuteAsync(replacementOperation, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new SagaException("Saga update failed", typeof(TSaga), instance.CorrelationId, exception);
            }
        }

        public Task Delete(SagaConsumeContext<TSaga> context)
        {
            return Delete(context.Saga.CorrelationId);
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }

        async Task<DynamicTableEntity> GetTableEntity(Guid correlationId)
        {
            Guid? rowKey = correlationId;
            var op = TableOperation.Retrieve<DynamicTableEntity>(_partitionKey, rowKey.ToString());
            var entity = await _table.ExecuteAsync(op);
            return entity.Result as DynamicTableEntity;
        }

        async Task<TSaga> Get(Guid correlationId)
        {
            var tableEntity = await GetTableEntity(correlationId).ConfigureAwait(false);
            return ConvertTableEntityToSagaInstance(tableEntity);
        }

        static TSaga ConvertTableEntityToSagaInstance(DynamicTableEntity tableEntity)
        {
            return tableEntity != null ? TableEntity.ConvertBack<TSaga>(tableEntity.Properties, new OperationContext()) : null;
        }

        async Task Delete(Guid correlationId)
        {
            var tableEntity = await GetTableEntity(correlationId).ConfigureAwait(false);
            await _table.ExecuteAsync(TableOperation.Delete(tableEntity));
        }
    }
}

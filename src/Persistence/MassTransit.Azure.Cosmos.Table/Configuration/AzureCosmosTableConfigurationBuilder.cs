namespace MassTransit.Azure.Cosmos.Table
{
    using System;
    using Microsoft.Azure.Cosmos.Table;


    class AzureCosmosTableConfigurationBuilder : IAzureCosmosTableConfigurator,
        IPartitionKeyHolder,
        ITableNameHolder,
        IFilterHolder,
        IBuilder
    {
        string _accessKey;
        string _accountName;
        string _connectionString;
        string _endpoint;
        Action<IMessageFilterConfigurator> _messageFilter;
        Func<string, AuditRecord, string> _partitionKeyStrategy;
        CloudStorageAccount _storageAccount;
        CloudTable _table;
        CloudTableClient _tableClient;
        string _tableName;

        public ITableNameHolder WithAccessKey(string accountName, string accessKey, string endpoint)
        {
            _accessKey = accessKey;
            _accountName = accountName;
            _endpoint = endpoint;
            _tableClient = new CloudTableClient(new Uri(_endpoint), new StorageCredentials(_accountName, _accessKey));
            return this;
        }

        public IPartitionKeyHolder WithTable(CloudTable table)
        {
            _table = table;
            return this;
        }

        public ITableNameHolder WithStorageAccount(CloudStorageAccount storageAccount)
        {
            _storageAccount = storageAccount;
            _tableClient = _storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            return this;
        }

        public ITableNameHolder WithConnectionString(string connectionString)
        {
            _connectionString = connectionString;
            _storageAccount = CloudStorageAccount.Parse(_connectionString);
            _tableClient = _storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            return this;
        }

        public AzureCosmosTableAuditBusObserver Build()
        {
            return new AzureCosmosTableAuditBusObserver(_table, _messageFilter, _partitionKeyStrategy);
        }

        public IBuilder WithMessageFilter(Action<IMessageFilterConfigurator> configureFilter)
        {
            _messageFilter = configureFilter;
            return this;
        }

        public IBuilder WithNoMessageFilter()
        {
            _messageFilter = null;
            return this;
        }

        public IFilterHolder WithCustomPartitionKey(Func<string, AuditRecord, string> partitionKeyStrategy)
        {
            _partitionKeyStrategy = partitionKeyStrategy;
            return this;
        }

        public IFilterHolder WithContextTypePartitionKeyStrategy()
        {
            _partitionKeyStrategy = (messageType, record) => record.ContextType;
            return this;
        }

        public IPartitionKeyHolder WithTableName(string tableName)
        {
            _tableName = tableName;
            _table = GetAuditCloudTable();
            return this;
        }

        CloudTable GetAuditCloudTable()
        {
            _table = _tableClient.GetTableReference(_tableName);
            _table.CreateIfNotExists();
            return _table;
        }
    }


    public interface IAzureCosmosTableConfigurator
    {
        ITableNameHolder WithConnectionString(string connectionString);
        ITableNameHolder WithAccessKey(string accountName, string accessKey, string endpoint);
        IPartitionKeyHolder WithTable(CloudTable table);
        ITableNameHolder WithStorageAccount(CloudStorageAccount storageAccount);
    }


    public interface ITableNameHolder
    {
        IPartitionKeyHolder WithTableName(string tableName);
    }


    public interface IPartitionKeyHolder
    {
        IFilterHolder WithCustomPartitionKey(Func<string, AuditRecord, string> partitionKeyStrategy);
        IFilterHolder WithContextTypePartitionKeyStrategy();
    }


    public interface IFilterHolder
    {
        IBuilder WithMessageFilter(Action<IMessageFilterConfigurator> configureFilter);
        IBuilder WithNoMessageFilter();
    }


    public interface IBuilder
    {
        AzureCosmosTableAuditBusObserver Build();
    }
}

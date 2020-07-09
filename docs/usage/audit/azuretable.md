# Azure Table Audit

Table audit supports both Cosmos DB Table API & Azure Storage account tables.

There are support for several different configuraiton options depending on your needs.

## Storage account configuration

### Supply storage account


> Uses [MassTransit.Azure.Cosmos.Table](https://nuget.org/packages/MassTransit.Azure.Cosmos.Table/)</br>
> Uses [MassTransit.Extensions.DependencyInjection](https://nuget.org/packages/MassTransit.Extensions.DependencyInjection/)

<<< @/docs/code/audit/AuditAzureTableWithStorageAccount.cs

### Supply your own table

> Uses [MassTransit.Azure.Cosmos.Table](https://nuget.org/packages/MassTransit.Azure.Cosmos.Table/)</br>
> Uses [MassTransit.Extensions.DependencyInjection](https://nuget.org/packages/MassTransit.Extensions.DependencyInjection/)


<<< @/docs/code/audit/AuditAzureTableWithTableSupplied.cs

## Partition Key Strategy

When using Azure Tables it is important to use the relevant partition key strategy to the likely queries you'll perform on the message data. The default partition key supplied is using the message context type (e.g "SEND" & "CONSUME"). However if you would like to override this, you can supply your own strategy by specifying this on configuration.

::: tip NOTE
Note: Please consider the official documentation for guidance on partition key strategy [here](https://docs.microsoft.com/en-us/rest/api/storageservices/designing-a-scalable-partitioning-strategy-for-azure-table-storage)
:::

> Uses [MassTransit.Azure.Cosmos.Table](https://nuget.org/packages/MassTransit.Azure.Cosmos.Table/)</br>
> Uses [MassTransit.Extensions.DependencyInjection](https://nuget.org/packages/MassTransit.Extensions.DependencyInjection/)


<<< @/docs/code/audit/AuditAzureTableWithCustomPartitionKey.cs

## Audit Filter

You can configure the message filter on auditing as below.

> Uses [MassTransit.Azure.Cosmos.Table](https://nuget.org/packages/MassTransit.Azure.Cosmos.Table/)</br>
> Uses [MassTransit.Extensions.DependencyInjection](https://nuget.org/packages/MassTransit.Extensions.DependencyInjection/)


<<< @/docs/code/audit/AuditAzureTableWithMessageTypeFilter.cs

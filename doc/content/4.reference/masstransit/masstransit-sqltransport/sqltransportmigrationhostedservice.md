---

title: SqlTransportMigrationHostedService

---

# SqlTransportMigrationHostedService

Namespace: MassTransit.SqlTransport

```csharp
public class SqlTransportMigrationHostedService : IHostedService
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlTransportMigrationHostedService](../masstransit-sqltransport/sqltransportmigrationhostedservice)<br/>
Implements IHostedService

## Constructors

### **SqlTransportMigrationHostedService(ISqlTransportDatabaseMigrator, ILogger\<SqlTransportMigrationHostedService\>, IOptions\<SqlTransportMigrationOptions\>, IOptions\<SqlTransportOptions\>)**

```csharp
public SqlTransportMigrationHostedService(ISqlTransportDatabaseMigrator migrator, ILogger<SqlTransportMigrationHostedService> logger, IOptions<SqlTransportMigrationOptions> options, IOptions<SqlTransportOptions> dbOptions)
```

#### Parameters

`migrator` [ISqlTransportDatabaseMigrator](../masstransit-sqltransport/isqltransportdatabasemigrator)<br/>

`logger` ILogger\<SqlTransportMigrationHostedService\><br/>

`options` IOptions\<SqlTransportMigrationOptions\><br/>

`dbOptions` IOptions\<SqlTransportOptions\><br/>

## Methods

### **StartAsync(CancellationToken)**

```csharp
public Task StartAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **StopAsync(CancellationToken)**

```csharp
public Task StopAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

---

title: ISqlTransportDatabaseMigrator

---

# ISqlTransportDatabaseMigrator

Namespace: MassTransit.SqlTransport

```csharp
public interface ISqlTransportDatabaseMigrator
```

## Methods

### **CreateDatabase(SqlTransportOptions, CancellationToken)**

```csharp
Task CreateDatabase(SqlTransportOptions options, CancellationToken cancellationToken)
```

#### Parameters

`options` [SqlTransportOptions](../masstransit/sqltransportoptions)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CreateSchemaIfNotExist(SqlTransportOptions, CancellationToken)**

```csharp
Task CreateSchemaIfNotExist(SqlTransportOptions options, CancellationToken cancellationToken)
```

#### Parameters

`options` [SqlTransportOptions](../masstransit/sqltransportoptions)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CreateInfrastructure(SqlTransportOptions, CancellationToken)**

```csharp
Task CreateInfrastructure(SqlTransportOptions options, CancellationToken cancellationToken)
```

#### Parameters

`options` [SqlTransportOptions](../masstransit/sqltransportoptions)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **DeleteDatabase(SqlTransportOptions, CancellationToken)**

```csharp
Task DeleteDatabase(SqlTransportOptions options, CancellationToken cancellationToken)
```

#### Parameters

`options` [SqlTransportOptions](../masstransit/sqltransportoptions)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

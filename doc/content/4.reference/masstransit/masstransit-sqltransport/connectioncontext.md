---

title: ConnectionContext

---

# ConnectionContext

Namespace: MassTransit.SqlTransport

```csharp
public interface ConnectionContext : PipeContext
```

Implements [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)

## Properties

### **HostAddress**

```csharp
public abstract Uri HostAddress { get; }
```

#### Property Value

Uri<br/>

### **Schema**

```csharp
public abstract string Schema { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IsolationLevel**

```csharp
public abstract IsolationLevel IsolationLevel { get; }
```

#### Property Value

IsolationLevel<br/>

## Methods

### **CreateClientContext(CancellationToken)**

```csharp
ClientContext CreateClientContext(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[ClientContext](../masstransit-sqltransport/clientcontext)<br/>

### **CreateConnection(CancellationToken)**

Create a database connection

```csharp
Task<ISqlTransportConnection> CreateConnection(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ISqlTransportConnection\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **DelayUntilMessageReady(Int64, TimeSpan, CancellationToken)**

```csharp
Task DelayUntilMessageReady(long queueId, TimeSpan timeout, CancellationToken cancellationToken)
```

#### Parameters

`queueId` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Query\<T\>(Func\<IDbConnection, IDbTransaction, Task\<T\>\>, CancellationToken)**

Executes a query within a transaction using an available connection

```csharp
Task<T> Query<T>(Func<IDbConnection, IDbTransaction, Task<T>> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`callback` [Func\<IDbConnection, IDbTransaction, Task\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

---

title: SharedConnectionContext

---

# SharedConnectionContext

Namespace: MassTransit.SqlTransport

```csharp
public class SharedConnectionContext : ProxyPipeContext, ConnectionContext, PipeContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ProxyPipeContext](../../masstransit-abstractions/masstransit-middleware/proxypipecontext) → [SharedConnectionContext](../masstransit-sqltransport/sharedconnectioncontext)<br/>
Implements [ConnectionContext](../masstransit-sqltransport/connectioncontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)

## Properties

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **HostAddress**

```csharp
public Uri HostAddress { get; }
```

#### Property Value

Uri<br/>

### **Schema**

```csharp
public string Schema { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IsolationLevel**

```csharp
public IsolationLevel IsolationLevel { get; }
```

#### Property Value

IsolationLevel<br/>

## Constructors

### **SharedConnectionContext(ConnectionContext, CancellationToken)**

```csharp
public SharedConnectionContext(ConnectionContext context, CancellationToken cancellationToken)
```

#### Parameters

`context` [ConnectionContext](../masstransit-sqltransport/connectioncontext)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **CreateClientContext(CancellationToken)**

```csharp
public ClientContext CreateClientContext(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[ClientContext](../masstransit-sqltransport/clientcontext)<br/>

### **CreateConnection(CancellationToken)**

```csharp
public Task<ISqlTransportConnection> CreateConnection(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ISqlTransportConnection\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **DelayUntilMessageReady(Int64, TimeSpan, CancellationToken)**

```csharp
public Task DelayUntilMessageReady(long queueId, TimeSpan timeout, CancellationToken cancellationToken)
```

#### Parameters

`queueId` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Query\<T\>(Func\<IDbConnection, IDbTransaction, Task\<T\>\>, CancellationToken)**

```csharp
public Task<T> Query<T>(Func<IDbConnection, IDbTransaction, Task<T>> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`callback` [Func\<IDbConnection, IDbTransaction, Task\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

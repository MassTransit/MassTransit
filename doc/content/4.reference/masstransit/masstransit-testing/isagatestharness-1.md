---

title: ISagaTestHarness<TSaga>

---

# ISagaTestHarness\<TSaga\>

Namespace: MassTransit.Testing

```csharp
public interface ISagaTestHarness<TSaga>
```

#### Type Parameters

`TSaga`<br/>

## Properties

### **Consumed**

```csharp
public abstract IReceivedMessageList Consumed { get; }
```

#### Property Value

[IReceivedMessageList](../masstransit-testing/ireceivedmessagelist)<br/>

### **Sagas**

```csharp
public abstract ISagaList<TSaga> Sagas { get; }
```

#### Property Value

[ISagaList\<TSaga\>](../masstransit-testing/isagalist-1)<br/>

### **Created**

```csharp
public abstract ISagaList<TSaga> Created { get; }
```

#### Property Value

[ISagaList\<TSaga\>](../masstransit-testing/isagalist-1)<br/>

## Methods

### **Exists(Guid, Nullable\<TimeSpan\>)**

Waits until a saga exists with the specified correlationId

```csharp
Task<Nullable<Guid>> Exists(Guid correlationId, Nullable<TimeSpan> timeout)
```

#### Parameters

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Match(Expression\<Func\<TSaga, Boolean\>\>, Nullable\<TimeSpan\>)**

Waits until at least one saga exists matching the specified filter

```csharp
Task<IList<Guid>> Match(Expression<Func<TSaga, bool>> filter, Nullable<TimeSpan> timeout)
```

#### Parameters

`filter` Expression\<Func\<TSaga, Boolean\>\><br/>

`timeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<IList\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **NotExists(Guid, Nullable\<TimeSpan\>)**

Waits until the saga matching the specified correlationId does NOT exist

```csharp
Task<Nullable<Guid>> NotExists(Guid correlationId, Nullable<TimeSpan> timeout)
```

#### Parameters

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

---

title: BaseSagaTestHarness<TSaga>

---

# BaseSagaTestHarness\<TSaga\>

Namespace: MassTransit.Testing.Implementations

```csharp
public abstract class BaseSagaTestHarness<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseSagaTestHarness\<TSaga\>](../masstransit-testing-implementations/basesagatestharness-1)

## Methods

### **Exists(Guid, Nullable\<TimeSpan\>)**

Waits until a saga exists with the specified correlationId

```csharp
public Task<Nullable<Guid>> Exists(Guid correlationId, Nullable<TimeSpan> timeout)
```

#### Parameters

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Match(Expression\<Func\<TSaga, Boolean\>\>, Nullable\<TimeSpan\>)**

Waits until at least one saga exists matching the specified filter

```csharp
public Task<IList<Guid>> Match(Expression<Func<TSaga, bool>> filter, Nullable<TimeSpan> timeout)
```

#### Parameters

`filter` Expression\<Func\<TSaga, Boolean\>\><br/>

`timeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<IList\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **NotExists(Guid, Nullable\<TimeSpan\>)**

Waits until the saga matching the specified correlationId does NOT exist

```csharp
public Task<Nullable<Guid>> NotExists(Guid correlationId, Nullable<TimeSpan> timeout)
```

#### Parameters

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

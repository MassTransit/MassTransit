---

title: ExtensionMethodsForSagas

---

# ExtensionMethodsForSagas

Namespace: MassTransit.Testing

```csharp
public static class ExtensionMethodsForSagas
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExtensionMethodsForSagas](../masstransit-testing/extensionmethodsforsagas)

## Methods

### **ShouldContainSaga\<TSaga\>(ISagaRepository\<TSaga\>, Guid, TimeSpan)**

```csharp
public static Task<Nullable<Guid>> ShouldContainSaga<TSaga>(ISagaRepository<TSaga> repository, Guid correlationId, TimeSpan timeout)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`repository` [ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ShouldContainSaga\<TSaga\>(ILoadSagaRepository\<TSaga\>, Guid, TimeSpan)**

```csharp
public static Task<Nullable<Guid>> ShouldContainSaga<TSaga>(ILoadSagaRepository<TSaga> repository, Guid correlationId, TimeSpan timeout)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`repository` [ILoadSagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/iloadsagarepository-1)<br/>

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ShouldContainSaga\<TSaga\>(IQuerySagaRepository\<TSaga\>, Guid, TimeSpan)**

```csharp
public static Task<Nullable<Guid>> ShouldContainSaga<TSaga>(IQuerySagaRepository<TSaga> repository, Guid correlationId, TimeSpan timeout)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`repository` [IQuerySagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/iquerysagarepository-1)<br/>

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ShouldContainSaga\<TSaga\>(ISagaRepository\<TSaga\>, Guid, Func\<TSaga, Boolean\>, TimeSpan)**

```csharp
public static Task<Nullable<Guid>> ShouldContainSaga<TSaga>(ISagaRepository<TSaga> repository, Guid correlationId, Func<TSaga, bool> condition, TimeSpan timeout)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`repository` [ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`condition` [Func\<TSaga, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ShouldContainSaga\<TSaga\>(ILoadSagaRepository\<TSaga\>, Guid, Func\<TSaga, Boolean\>, TimeSpan)**

```csharp
public static Task<Nullable<Guid>> ShouldContainSaga<TSaga>(ILoadSagaRepository<TSaga> repository, Guid correlationId, Func<TSaga, bool> condition, TimeSpan timeout)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`repository` [ILoadSagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/iloadsagarepository-1)<br/>

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`condition` [Func\<TSaga, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ShouldNotContainSaga\<TSaga\>(ISagaRepository\<TSaga\>, Guid, TimeSpan)**

```csharp
public static Task<Nullable<Guid>> ShouldNotContainSaga<TSaga>(ISagaRepository<TSaga> repository, Guid correlationId, TimeSpan timeout)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`repository` [ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ShouldNotContainSaga\<TSaga\>(ILoadSagaRepository\<TSaga\>, Guid, TimeSpan)**

```csharp
public static Task<Nullable<Guid>> ShouldNotContainSaga<TSaga>(ILoadSagaRepository<TSaga> repository, Guid correlationId, TimeSpan timeout)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`repository` [ILoadSagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/iloadsagarepository-1)<br/>

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ShouldNotContainSaga\<TSaga\>(IQuerySagaRepository\<TSaga\>, Guid, TimeSpan)**

```csharp
public static Task<Nullable<Guid>> ShouldNotContainSaga<TSaga>(IQuerySagaRepository<TSaga> repository, Guid correlationId, TimeSpan timeout)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`repository` [IQuerySagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/iquerysagarepository-1)<br/>

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ShouldContainSaga\<TSaga\>(ISagaRepository\<TSaga\>, Expression\<Func\<TSaga, Boolean\>\>, TimeSpan)**

```csharp
public static Task<Nullable<Guid>> ShouldContainSaga<TSaga>(ISagaRepository<TSaga> repository, Expression<Func<TSaga, bool>> filter, TimeSpan timeout)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`repository` [ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`filter` Expression\<Func\<TSaga, Boolean\>\><br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ShouldContainSaga\<TSaga\>(IQuerySagaRepository\<TSaga\>, Expression\<Func\<TSaga, Boolean\>\>, TimeSpan)**

```csharp
public static Task<Nullable<Guid>> ShouldContainSaga<TSaga>(IQuerySagaRepository<TSaga> repository, Expression<Func<TSaga, bool>> filter, TimeSpan timeout)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`repository` [IQuerySagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/iquerysagarepository-1)<br/>

`filter` Expression\<Func\<TSaga, Boolean\>\><br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

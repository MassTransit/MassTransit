---

title: OutboxContext

---

# OutboxContext

Namespace: MassTransit.Middleware.InMemoryOutbox

The context for an outbox instance as part of consume context. Used to signal the completion of
 the consume, and store any Task factories that should be created.

```csharp
public interface OutboxContext
```

## Properties

### **ClearToSend**

Returns an awaitable task that is completed when it is clear to send messages

```csharp
public abstract Task ClearToSend { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

## Methods

### **Add(Func\<Task\>)**

Adds a method to be invoked once the outbox is ready to be sent

```csharp
Task Add(Func<Task> method)
```

#### Parameters

`method` [Func\<Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ExecutePendingActions(Boolean)**

Execute all the pending outbox operations (success case)

```csharp
Task ExecutePendingActions(bool concurrentMessageDelivery)
```

#### Parameters

`concurrentMessageDelivery` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **DiscardPendingActions()**

Discard any pending outbox operations, and cancel any scheduled messages

```csharp
Task DiscardPendingActions()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

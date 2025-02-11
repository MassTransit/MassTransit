---

title: ConcurrencyLimiter

---

# ConcurrencyLimiter

Namespace: MassTransit.Middleware

A concurrency limiter (using a semaphore) which can be shared, and adjusted using a management
 endpoint.

```csharp
public class ConcurrencyLimiter : IConcurrencyLimiter, IConsumer<SetConcurrencyLimit>, IConsumer
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConcurrencyLimiter](../masstransit-middleware/concurrencylimiter)<br/>
Implements [IConcurrencyLimiter](../masstransit-middleware/iconcurrencylimiter), [IConsumer\<SetConcurrencyLimit\>](../../masstransit-abstractions/masstransit/iconsumer-1), [IConsumer](../../masstransit-abstractions/masstransit/iconsumer)

## Constructors

### **ConcurrencyLimiter(Int32, String)**

```csharp
public ConcurrencyLimiter(int concurrencyLimit, string id)
```

#### Parameters

`concurrencyLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`id` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Wait(CancellationToken)**

```csharp
public Task Wait(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Release()**

```csharp
public void Release()
```

### **Consume(ConsumeContext\<SetConcurrencyLimit\>)**

```csharp
public Task Consume(ConsumeContext<SetConcurrencyLimit> context)
```

#### Parameters

`context` [ConsumeContext\<SetConcurrencyLimit\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

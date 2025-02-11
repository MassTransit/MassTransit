---

title: IConcurrencyLimiter

---

# IConcurrencyLimiter

Namespace: MassTransit.Middleware

```csharp
public interface IConcurrencyLimiter : IConsumer<SetConcurrencyLimit>, IConsumer
```

Implements [IConsumer\<SetConcurrencyLimit\>](../../masstransit-abstractions/masstransit/iconsumer-1), [IConsumer](../../masstransit-abstractions/masstransit/iconsumer)

## Properties

### **Available**

```csharp
public abstract int Available { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Limit**

```csharp
public abstract int Limit { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **Wait(CancellationToken)**

```csharp
Task Wait(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Release()**

```csharp
void Release()
```

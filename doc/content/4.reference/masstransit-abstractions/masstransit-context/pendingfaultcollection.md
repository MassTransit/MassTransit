---

title: PendingFaultCollection

---

# PendingFaultCollection

Namespace: MassTransit.Context

```csharp
public class PendingFaultCollection
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PendingFaultCollection](../masstransit-context/pendingfaultcollection)

## Constructors

### **PendingFaultCollection()**

```csharp
public PendingFaultCollection()
```

## Methods

### **Add\<T\>(ConsumeContext\<T\>, TimeSpan, String, Exception)**

```csharp
public void Add<T>(ConsumeContext<T> context, TimeSpan elapsed, string consumerType, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>

`elapsed` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **Notify(ConsumeContext)**

```csharp
public Task Notify(ConsumeContext consumeContext)
```

#### Parameters

`consumeContext` [ConsumeContext](../masstransit/consumecontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

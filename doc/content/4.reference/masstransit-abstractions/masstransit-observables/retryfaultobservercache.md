---

title: RetryFaultObserverCache

---

# RetryFaultObserverCache

Namespace: MassTransit.Observables

```csharp
public class RetryFaultObserverCache
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RetryFaultObserverCache](../masstransit-observables/retryfaultobservercache)

## Constructors

### **RetryFaultObserverCache()**

```csharp
public RetryFaultObserverCache()
```

## Methods

### **RetryFault(IRetryObserver, RetryContext, Type)**

```csharp
public static Task RetryFault(IRetryObserver observer, RetryContext context, Type contextType)
```

#### Parameters

`observer` [IRetryObserver](../masstransit/iretryobserver)<br/>

`context` [RetryContext](../masstransit/retrycontext)<br/>

`contextType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

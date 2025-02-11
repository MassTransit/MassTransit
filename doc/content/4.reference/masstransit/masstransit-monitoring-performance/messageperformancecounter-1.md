---

title: MessagePerformanceCounter<TMessage>

---

# MessagePerformanceCounter\<TMessage\>

Namespace: MassTransit.Monitoring.Performance

```csharp
public class MessagePerformanceCounter<TMessage> : IDisposable, IMessagePerformanceCounter
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessagePerformanceCounter\<TMessage\>](../masstransit-monitoring-performance/messageperformancecounter-1)<br/>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable), [IMessagePerformanceCounter](../masstransit-monitoring-performance/imessageperformancecounter)

## Constructors

### **MessagePerformanceCounter(ICounterFactory)**

```csharp
public MessagePerformanceCounter(ICounterFactory factory)
```

#### Parameters

`factory` [ICounterFactory](../masstransit-monitoring-performance/icounterfactory)<br/>

## Methods

### **Dispose()**

```csharp
public void Dispose()
```

### **Consumed(TimeSpan)**

```csharp
public void Consumed(TimeSpan duration)
```

#### Parameters

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **ConsumeFaulted(TimeSpan)**

```csharp
public void ConsumeFaulted(TimeSpan duration)
```

#### Parameters

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **Sent()**

```csharp
public void Sent()
```

### **Published()**

```csharp
public void Published()
```

### **PublishFaulted()**

```csharp
public void PublishFaulted()
```

### **SendFaulted()**

```csharp
public void SendFaulted()
```

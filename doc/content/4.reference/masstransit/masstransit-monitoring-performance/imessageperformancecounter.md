---

title: IMessagePerformanceCounter

---

# IMessagePerformanceCounter

Namespace: MassTransit.Monitoring.Performance

```csharp
public interface IMessagePerformanceCounter
```

## Methods

### **Consumed(TimeSpan)**

A message was consumed, including the consume duration

```csharp
void Consumed(TimeSpan duration)
```

#### Parameters

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **ConsumeFaulted(TimeSpan)**

A message faulted while being consumed

```csharp
void ConsumeFaulted(TimeSpan duration)
```

#### Parameters

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **Sent()**

A message was sent

```csharp
void Sent()
```

### **Published()**

A message was published

```csharp
void Published()
```

### **PublishFaulted()**

A publish faulted

```csharp
void PublishFaulted()
```

### **SendFaulted()**

A send faulted

```csharp
void SendFaulted()
```

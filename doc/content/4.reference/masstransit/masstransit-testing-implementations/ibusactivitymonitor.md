---

title: IBusActivityMonitor

---

# IBusActivityMonitor

Namespace: MassTransit.Testing.Implementations

Represents a monitor for bus activity, allowing awaiting an inactive bus state.

```csharp
public interface IBusActivityMonitor
```

## Methods

### **AwaitBusInactivity()**

```csharp
Task AwaitBusInactivity()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **AwaitBusInactivity(TimeSpan)**

```csharp
Task<bool> AwaitBusInactivity(TimeSpan timeout)
```

#### Parameters

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **AwaitBusInactivity(CancellationToken)**

```csharp
Task AwaitBusInactivity(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

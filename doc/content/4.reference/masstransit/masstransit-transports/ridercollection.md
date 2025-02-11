---

title: RiderCollection

---

# RiderCollection

Namespace: MassTransit.Transports

```csharp
public class RiderCollection : Agent, IAgent, IRiderCollection
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Agent](../../masstransit-abstractions/masstransit-middleware/agent) → [RiderCollection](../masstransit-transports/ridercollection)<br/>
Implements [IAgent](../../masstransit-abstractions/masstransit/iagent), [IRiderCollection](../masstransit-transports/iridercollection)

## Properties

### **Ready**

```csharp
public Task Ready { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Completed**

```csharp
public Task Completed { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Stopping**

```csharp
public CancellationToken Stopping { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **Stopped**

```csharp
public CancellationToken Stopped { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Constructors

### **RiderCollection()**

```csharp
public RiderCollection()
```

## Methods

### **Get(String)**

```csharp
public IRider Get(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IRider](../../masstransit-abstractions/masstransit-transports/irider)<br/>

### **Add(String, IRiderControl)**

```csharp
public void Add(string name, IRiderControl rider)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`rider` [IRiderControl](../../masstransit-abstractions/masstransit-transports/iridercontrol)<br/>

### **StartRiders(CancellationToken)**

```csharp
public HostRiderHandle[] StartRiders(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[HostRiderHandle[]](../../masstransit-abstractions/masstransit-transports/hostriderhandle)<br/>

### **StartRider(String, CancellationToken)**

```csharp
public HostRiderHandle StartRider(string name, CancellationToken cancellationToken)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[HostRiderHandle](../../masstransit-abstractions/masstransit-transports/hostriderhandle)<br/>

### **CheckEndpointHealth()**

```csharp
public IEnumerable<EndpointHealthResult> CheckEndpointHealth()
```

#### Returns

[IEnumerable\<EndpointHealthResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **StopAgent(StopContext)**

```csharp
protected Task StopAgent(StopContext context)
```

#### Parameters

`context` [StopContext](../../masstransit-abstractions/masstransit/stopcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

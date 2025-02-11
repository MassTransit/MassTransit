---

title: IRiderCollection

---

# IRiderCollection

Namespace: MassTransit.Transports

```csharp
public interface IRiderCollection : IAgent
```

Implements [IAgent](../../masstransit-abstractions/masstransit/iagent)

## Methods

### **Get(String)**

```csharp
IRider Get(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IRider](../../masstransit-abstractions/masstransit-transports/irider)<br/>

### **Add(String, IRiderControl)**

```csharp
void Add(string name, IRiderControl rider)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`rider` [IRiderControl](../../masstransit-abstractions/masstransit-transports/iridercontrol)<br/>

### **StartRiders(CancellationToken)**

```csharp
HostRiderHandle[] StartRiders(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[HostRiderHandle[]](../../masstransit-abstractions/masstransit-transports/hostriderhandle)<br/>

### **StartRider(String, CancellationToken)**

```csharp
HostRiderHandle StartRider(string name, CancellationToken cancellationToken)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[HostRiderHandle](../../masstransit-abstractions/masstransit-transports/hostriderhandle)<br/>

### **CheckEndpointHealth()**

```csharp
IEnumerable<EndpointHealthResult> CheckEndpointHealth()
```

#### Returns

[IEnumerable\<EndpointHealthResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

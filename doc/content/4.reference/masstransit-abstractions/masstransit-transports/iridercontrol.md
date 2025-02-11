---

title: IRiderControl

---

# IRiderControl

Namespace: MassTransit.Transports

```csharp
public interface IRiderControl : IRider
```

Implements [IRider](../masstransit-transports/irider)

## Methods

### **Start(CancellationToken)**

```csharp
RiderHandle Start(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[RiderHandle](../masstransit-transports/riderhandle)<br/>

### **CheckEndpointHealth()**

```csharp
IEnumerable<EndpointHealthResult> CheckEndpointHealth()
```

#### Returns

[IEnumerable\<EndpointHealthResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

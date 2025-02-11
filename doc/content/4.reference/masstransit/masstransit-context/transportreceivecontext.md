---

title: TransportReceiveContext

---

# TransportReceiveContext

Namespace: MassTransit.Context

```csharp
public interface TransportReceiveContext
```

## Methods

### **GetTransportProperties()**

Write any transport-specific properties to the dictionary so that they can be
 restored on subsequent outgoing messages (scheduled)

```csharp
IDictionary<string, object> GetTransportProperties()
```

#### Returns

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

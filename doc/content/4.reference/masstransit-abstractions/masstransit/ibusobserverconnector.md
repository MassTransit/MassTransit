---

title: IBusObserverConnector

---

# IBusObserverConnector

Namespace: MassTransit

```csharp
public interface IBusObserverConnector
```

## Methods

### **ConnectBusObserver(IBusObserver)**

Connects a bus observer to the bus to observe lifecycle events on the bus

```csharp
ConnectHandle ConnectBusObserver(IBusObserver observer)
```

#### Parameters

`observer` [IBusObserver](../masstransit/ibusobserver)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>

---

title: IBusInstance<TBus>

---

# IBusInstance\<TBus\>

Namespace: MassTransit.Transports

```csharp
public interface IBusInstance<TBus> : IBusInstance, IReceiveEndpointConnector
```

#### Type Parameters

`TBus`<br/>

Implements [IBusInstance](../masstransit-transports/ibusinstance), [IReceiveEndpointConnector](../masstransit/ireceiveendpointconnector)

## Properties

### **Bus**

```csharp
public abstract TBus Bus { get; }
```

#### Property Value

TBus<br/>

### **BusInstance**

The original bus instance (since this is wrapped inside a multi-bus instance

```csharp
public abstract IBusInstance BusInstance { get; }
```

#### Property Value

[IBusInstance](../masstransit-transports/ibusinstance)<br/>

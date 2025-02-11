---

title: RoundRobinReceiverLoadBalancer<T>

---

# RoundRobinReceiverLoadBalancer\<T\>

Namespace: MassTransit.Transports.Fabric

```csharp
public class RoundRobinReceiverLoadBalancer<T> : IReceiverLoadBalancer<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoundRobinReceiverLoadBalancer\<T\>](../masstransit-transports-fabric/roundrobinreceiverloadbalancer-1)<br/>
Implements [IReceiverLoadBalancer\<T\>](../masstransit-transports-fabric/ireceiverloadbalancer-1)

## Constructors

### **RoundRobinReceiverLoadBalancer(IMessageReceiver`1[])**

```csharp
public RoundRobinReceiverLoadBalancer(IMessageReceiver`1[] receivers)
```

#### Parameters

`receivers` [IMessageReceiver`1[]](../masstransit-transports-fabric/imessagereceiver-1)<br/>

## Methods

### **SelectReceiver(T)**

```csharp
public IMessageReceiver<T> SelectReceiver(T message)
```

#### Parameters

`message` T<br/>

#### Returns

[IMessageReceiver\<T\>](../masstransit-transports-fabric/imessagereceiver-1)<br/>

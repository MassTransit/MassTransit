---

title: SingleReceiverLoadBalancer<T>

---

# SingleReceiverLoadBalancer\<T\>

Namespace: MassTransit.Transports.Fabric

```csharp
public class SingleReceiverLoadBalancer<T> : IReceiverLoadBalancer<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SingleReceiverLoadBalancer\<T\>](../masstransit-transports-fabric/singlereceiverloadbalancer-1)<br/>
Implements [IReceiverLoadBalancer\<T\>](../masstransit-transports-fabric/ireceiverloadbalancer-1)

## Constructors

### **SingleReceiverLoadBalancer(IMessageReceiver\<T\>)**

```csharp
public SingleReceiverLoadBalancer(IMessageReceiver<T> receiver)
```

#### Parameters

`receiver` [IMessageReceiver\<T\>](../masstransit-transports-fabric/imessagereceiver-1)<br/>

## Methods

### **SelectReceiver(T)**

```csharp
public IMessageReceiver<T> SelectReceiver(T message)
```

#### Parameters

`message` T<br/>

#### Returns

[IMessageReceiver\<T\>](../masstransit-transports-fabric/imessagereceiver-1)<br/>

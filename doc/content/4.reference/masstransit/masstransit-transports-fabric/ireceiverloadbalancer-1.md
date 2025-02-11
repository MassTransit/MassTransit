---

title: IReceiverLoadBalancer<T>

---

# IReceiverLoadBalancer\<T\>

Namespace: MassTransit.Transports.Fabric

```csharp
public interface IReceiverLoadBalancer<T>
```

#### Type Parameters

`T`<br/>

## Methods

### **SelectReceiver(T)**

```csharp
IMessageReceiver<T> SelectReceiver(T message)
```

#### Parameters

`message` T<br/>

#### Returns

[IMessageReceiver\<T\>](../masstransit-transports-fabric/imessagereceiver-1)<br/>

---

title: IMessageFabricObserverConnector<TContext>

---

# IMessageFabricObserverConnector\<TContext\>

Namespace: MassTransit.Transports.Fabric

```csharp
public interface IMessageFabricObserverConnector<TContext>
```

#### Type Parameters

`TContext`<br/>

## Methods

### **ConnectMessageFabricObserver(IMessageFabricObserver\<TContext\>)**

```csharp
ConnectHandle ConnectMessageFabricObserver(IMessageFabricObserver<TContext> observer)
```

#### Parameters

`observer` [IMessageFabricObserver\<TContext\>](../masstransit-transports-fabric/imessagefabricobserver-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

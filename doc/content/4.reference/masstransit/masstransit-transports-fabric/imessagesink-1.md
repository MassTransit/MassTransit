---

title: IMessageSink<T>

---

# IMessageSink\<T\>

Namespace: MassTransit.Transports.Fabric

```csharp
public interface IMessageSink<T> : IProbeSite
```

#### Type Parameters

`T`<br/>

Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **Deliver(DeliveryContext\<T\>)**

```csharp
Task Deliver(DeliveryContext<T> context)
```

#### Parameters

`context` [DeliveryContext\<T\>](../masstransit-transports-fabric/deliverycontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

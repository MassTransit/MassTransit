---

title: HandlerPipeSpecification<T>

---

# HandlerPipeSpecification\<T\>

Namespace: MassTransit.Configuration

Adds a message handler to the consuming pipe builder

```csharp
public class HandlerPipeSpecification<T> : IPipeSpecification<ConsumeContext<T>>, ISpecification
```

#### Type Parameters

`T`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [HandlerPipeSpecification\<T\>](../masstransit-configuration/handlerpipespecification-1)<br/>
Implements [IPipeSpecification\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **HandlerPipeSpecification(MessageHandler\<T\>)**

```csharp
public HandlerPipeSpecification(MessageHandler<T> handler)
```

#### Parameters

`handler` [MessageHandler\<T\>](../../masstransit-abstractions/masstransit/messagehandler-1)<br/>

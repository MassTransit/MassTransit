---

title: ObserverPipeSpecification<T>

---

# ObserverPipeSpecification\<T\>

Namespace: MassTransit.Configuration

Adds a message handler to the consuming pipe builder

```csharp
public class ObserverPipeSpecification<T> : IPipeSpecification<ConsumeContext<T>>, ISpecification
```

#### Type Parameters

`T`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ObserverPipeSpecification\<T\>](../masstransit-configuration/observerpipespecification-1)<br/>
Implements [IPipeSpecification\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **ObserverPipeSpecification(IObserver\<ConsumeContext\<T\>\>)**

```csharp
public ObserverPipeSpecification(IObserver<ConsumeContext<T>> observer)
```

#### Parameters

`observer` [IObserver\<ConsumeContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.iobserver-1)<br/>

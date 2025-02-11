---

title: ObserverConfigurator<TMessage>

---

# ObserverConfigurator\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class ObserverConfigurator<TMessage> : IObserverConfigurator<TMessage>, IConsumeConfigurator, IPipeConfigurator<ConsumeContext<TMessage>>, IReceiveEndpointSpecification, ISpecification
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ObserverConfigurator\<TMessage\>](../masstransit-configuration/observerconfigurator-1)<br/>
Implements [IObserverConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/iobserverconfigurator-1), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator), [IPipeConfigurator\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IReceiveEndpointSpecification](../../masstransit-abstractions/masstransit/ireceiveendpointspecification), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **ObserverConfigurator(IObserver\<ConsumeContext\<TMessage\>\>)**

```csharp
public ObserverConfigurator(IObserver<ConsumeContext<TMessage>> observer)
```

#### Parameters

`observer` [IObserver\<ConsumeContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.iobserver-1)<br/>

## Methods

### **AddPipeSpecification(IPipeSpecification\<ConsumeContext\<TMessage\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Configure(IReceiveEndpointBuilder)**

```csharp
public void Configure(IReceiveEndpointBuilder builder)
```

#### Parameters

`builder` [IReceiveEndpointBuilder](../../masstransit-abstractions/masstransit-configuration/ireceiveendpointbuilder)<br/>

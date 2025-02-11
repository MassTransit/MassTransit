---

title: ConsumeTransformSpecification<TMessage>

---

# ConsumeTransformSpecification\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class ConsumeTransformSpecification<TMessage> : TransformSpecification<TMessage>, ITransformConfigurator<TMessage>, IConsumeTransformSpecification<TMessage>, IPipeSpecification<ConsumeContext<TMessage>>, ISpecification
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransformSpecification\<TMessage\>](../masstransit-configuration/transformspecification-1) → [ConsumeTransformSpecification\<TMessage\>](../masstransit-configuration/consumetransformspecification-1)<br/>
Implements [ITransformConfigurator\<TMessage\>](../masstransit/itransformconfigurator-1), [IConsumeTransformSpecification\<TMessage\>](../masstransit-configuration/iconsumetransformspecification-1), [IPipeSpecification\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Replace**

```csharp
public bool Replace { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **ConsumeTransformSpecification()**

```csharp
public ConsumeTransformSpecification()
```

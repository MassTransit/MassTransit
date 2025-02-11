---

title: SendTransformSpecification<TMessage>

---

# SendTransformSpecification\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class SendTransformSpecification<TMessage> : TransformSpecification<TMessage>, ITransformConfigurator<TMessage>, ISendTransformSpecification<TMessage>, IPipeSpecification<SendContext<TMessage>>, ISpecification
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransformSpecification\<TMessage\>](../masstransit-configuration/transformspecification-1) → [SendTransformSpecification\<TMessage\>](../masstransit-configuration/sendtransformspecification-1)<br/>
Implements [ITransformConfigurator\<TMessage\>](../masstransit/itransformconfigurator-1), [ISendTransformSpecification\<TMessage\>](../masstransit-configuration/isendtransformspecification-1), [IPipeSpecification\<SendContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

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

### **SendTransformSpecification()**

```csharp
public SendTransformSpecification()
```

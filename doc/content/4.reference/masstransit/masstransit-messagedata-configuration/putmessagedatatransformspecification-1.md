---

title: PutMessageDataTransformSpecification<TMessage>

---

# PutMessageDataTransformSpecification\<TMessage\>

Namespace: MassTransit.MessageData.Configuration

```csharp
public class PutMessageDataTransformSpecification<TMessage> : TransformSpecification<TMessage>, ITransformConfigurator<TMessage>, ISendTransformSpecification<TMessage>, IPipeSpecification<SendContext<TMessage>>, ISpecification
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransformSpecification\<TMessage\>](../masstransit-configuration/transformspecification-1) → [PutMessageDataTransformSpecification\<TMessage\>](../masstransit-messagedata-configuration/putmessagedatatransformspecification-1)<br/>
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

### **PutMessageDataTransformSpecification(IMessageDataRepository, IEnumerable\<Type\>)**

```csharp
public PutMessageDataTransformSpecification(IMessageDataRepository repository, IEnumerable<Type> knownTypes)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

`knownTypes` [IEnumerable\<Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Methods

### **TryGetSendTopology(IMessageSendTopology\<TMessage\>)**

```csharp
public bool TryGetSendTopology(out IMessageSendTopology<TMessage> topology)
```

#### Parameters

`topology` [IMessageSendTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessagesendtopology-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetConverter(IPropertyConverter\<TMessage, TMessage\>)**

```csharp
public bool TryGetConverter(out IPropertyConverter<TMessage, TMessage> converter)
```

#### Parameters

`converter` [IPropertyConverter\<TMessage, TMessage\>](../masstransit-initializers/ipropertyconverter-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

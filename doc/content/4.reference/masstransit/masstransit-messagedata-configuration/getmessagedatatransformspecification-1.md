---

title: GetMessageDataTransformSpecification<TMessage>

---

# GetMessageDataTransformSpecification\<TMessage\>

Namespace: MassTransit.MessageData.Configuration

```csharp
public class GetMessageDataTransformSpecification<TMessage> : TransformSpecification<TMessage>, ITransformConfigurator<TMessage>, IConsumeTransformSpecification<TMessage>, IPipeSpecification<ConsumeContext<TMessage>>, ISpecification, IExecuteTransformSpecification<TMessage>, IPipeSpecification<ExecuteContext<TMessage>>, ICompensateTransformSpecification<TMessage>, IPipeSpecification<CompensateContext<TMessage>>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransformSpecification\<TMessage\>](../masstransit-configuration/transformspecification-1) → [GetMessageDataTransformSpecification\<TMessage\>](../masstransit-messagedata-configuration/getmessagedatatransformspecification-1)<br/>
Implements [ITransformConfigurator\<TMessage\>](../masstransit/itransformconfigurator-1), [IConsumeTransformSpecification\<TMessage\>](../masstransit-configuration/iconsumetransformspecification-1), [IPipeSpecification\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IExecuteTransformSpecification\<TMessage\>](../masstransit-configuration/iexecutetransformspecification-1), [IPipeSpecification\<ExecuteContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ICompensateTransformSpecification\<TMessage\>](../masstransit-configuration/icompensatetransformspecification-1), [IPipeSpecification\<CompensateContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)

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

### **GetMessageDataTransformSpecification(IMessageDataRepository, IEnumerable\<Type\>)**

```csharp
public GetMessageDataTransformSpecification(IMessageDataRepository repository, IEnumerable<Type> knownTypes)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

`knownTypes` [IEnumerable\<Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Methods

### **TryGetConsumeTopology(IMessageConsumeTopology\<TMessage\>)**

```csharp
public bool TryGetConsumeTopology(out IMessageConsumeTopology<TMessage> topology)
```

#### Parameters

`topology` [IMessageConsumeTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessageconsumetopology-1)<br/>

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

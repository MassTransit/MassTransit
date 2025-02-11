---

title: GetMessageDataObjectArrayTransformConfiguration<TInput, TProperty, TElement>

---

# GetMessageDataObjectArrayTransformConfiguration\<TInput, TProperty, TElement\>

Namespace: MassTransit.MessageData.Configuration

```csharp
public class GetMessageDataObjectArrayTransformConfiguration<TInput, TProperty, TElement> : IMessageDataTransformConfiguration<TInput>
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

`TElement`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [GetMessageDataObjectArrayTransformConfiguration\<TInput, TProperty, TElement\>](../masstransit-messagedata-configuration/getmessagedataobjectarraytransformconfiguration-3)<br/>
Implements [IMessageDataTransformConfiguration\<TInput\>](../masstransit-messagedata-configuration/imessagedatatransformconfiguration-1)

## Constructors

### **GetMessageDataObjectArrayTransformConfiguration(IMessageDataRepository, IEnumerable\<Type\>, PropertyInfo)**

```csharp
public GetMessageDataObjectArrayTransformConfiguration(IMessageDataRepository repository, IEnumerable<Type> knownTypes, PropertyInfo property)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

`knownTypes` [IEnumerable\<Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`property` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Methods

### **Apply(ITransformConfigurator\<TInput\>)**

```csharp
public void Apply(ITransformConfigurator<TInput> configurator)
```

#### Parameters

`configurator` [ITransformConfigurator\<TInput\>](../masstransit/itransformconfigurator-1)<br/>

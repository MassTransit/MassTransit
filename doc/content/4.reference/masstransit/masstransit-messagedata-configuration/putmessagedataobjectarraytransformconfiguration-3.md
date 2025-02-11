---

title: PutMessageDataObjectArrayTransformConfiguration<TInput, TProperty, TElement>

---

# PutMessageDataObjectArrayTransformConfiguration\<TInput, TProperty, TElement\>

Namespace: MassTransit.MessageData.Configuration

```csharp
public class PutMessageDataObjectArrayTransformConfiguration<TInput, TProperty, TElement> : IMessageDataTransformConfiguration<TInput>
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

`TElement`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PutMessageDataObjectArrayTransformConfiguration\<TInput, TProperty, TElement\>](../masstransit-messagedata-configuration/putmessagedataobjectarraytransformconfiguration-3)<br/>
Implements [IMessageDataTransformConfiguration\<TInput\>](../masstransit-messagedata-configuration/imessagedatatransformconfiguration-1)

## Constructors

### **PutMessageDataObjectArrayTransformConfiguration(IMessageDataRepository, IEnumerable\<Type\>, PropertyInfo)**

```csharp
public PutMessageDataObjectArrayTransformConfiguration(IMessageDataRepository repository, IEnumerable<Type> knownTypes, PropertyInfo property)
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

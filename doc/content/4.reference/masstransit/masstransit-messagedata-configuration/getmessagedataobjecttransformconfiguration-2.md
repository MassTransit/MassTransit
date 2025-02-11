---

title: GetMessageDataObjectTransformConfiguration<TInput, TProperty>

---

# GetMessageDataObjectTransformConfiguration\<TInput, TProperty\>

Namespace: MassTransit.MessageData.Configuration

```csharp
public class GetMessageDataObjectTransformConfiguration<TInput, TProperty> : IMessageDataTransformConfiguration<TInput>
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [GetMessageDataObjectTransformConfiguration\<TInput, TProperty\>](../masstransit-messagedata-configuration/getmessagedataobjecttransformconfiguration-2)<br/>
Implements [IMessageDataTransformConfiguration\<TInput\>](../masstransit-messagedata-configuration/imessagedatatransformconfiguration-1)

## Constructors

### **GetMessageDataObjectTransformConfiguration(IMessageDataRepository, IEnumerable\<Type\>, PropertyInfo)**

```csharp
public GetMessageDataObjectTransformConfiguration(IMessageDataRepository repository, IEnumerable<Type> knownTypes, PropertyInfo property)
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

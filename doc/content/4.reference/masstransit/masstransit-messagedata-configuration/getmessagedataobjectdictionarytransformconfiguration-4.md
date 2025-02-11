---

title: GetMessageDataObjectDictionaryTransformConfiguration<TInput, TProperty, TKey, TValue>

---

# GetMessageDataObjectDictionaryTransformConfiguration\<TInput, TProperty, TKey, TValue\>

Namespace: MassTransit.MessageData.Configuration

```csharp
public class GetMessageDataObjectDictionaryTransformConfiguration<TInput, TProperty, TKey, TValue> : IMessageDataTransformConfiguration<TInput>
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

`TKey`<br/>

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [GetMessageDataObjectDictionaryTransformConfiguration\<TInput, TProperty, TKey, TValue\>](../masstransit-messagedata-configuration/getmessagedataobjectdictionarytransformconfiguration-4)<br/>
Implements [IMessageDataTransformConfiguration\<TInput\>](../masstransit-messagedata-configuration/imessagedatatransformconfiguration-1)

## Constructors

### **GetMessageDataObjectDictionaryTransformConfiguration(IMessageDataRepository, IEnumerable\<Type\>, PropertyInfo)**

```csharp
public GetMessageDataObjectDictionaryTransformConfiguration(IMessageDataRepository repository, IEnumerable<Type> knownTypes, PropertyInfo property)
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

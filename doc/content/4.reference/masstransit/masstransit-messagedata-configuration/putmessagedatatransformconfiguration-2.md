---

title: PutMessageDataTransformConfiguration<TInput, TValue>

---

# PutMessageDataTransformConfiguration\<TInput, TValue\>

Namespace: MassTransit.MessageData.Configuration

```csharp
public class PutMessageDataTransformConfiguration<TInput, TValue> : IMessageDataTransformConfiguration<TInput>
```

#### Type Parameters

`TInput`<br/>

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PutMessageDataTransformConfiguration\<TInput, TValue\>](../masstransit-messagedata-configuration/putmessagedatatransformconfiguration-2)<br/>
Implements [IMessageDataTransformConfiguration\<TInput\>](../masstransit-messagedata-configuration/imessagedatatransformconfiguration-1)

## Constructors

### **PutMessageDataTransformConfiguration(IMessageDataRepository, PropertyInfo)**

```csharp
public PutMessageDataTransformConfiguration(IMessageDataRepository repository, PropertyInfo property)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

`property` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Methods

### **Apply(ITransformConfigurator\<TInput\>)**

```csharp
public void Apply(ITransformConfigurator<TInput> configurator)
```

#### Parameters

`configurator` [ITransformConfigurator\<TInput\>](../masstransit/itransformconfigurator-1)<br/>

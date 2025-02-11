---

title: GetMessageDataTransformConfiguration<TInput, TValue>

---

# GetMessageDataTransformConfiguration\<TInput, TValue\>

Namespace: MassTransit.MessageData.Configuration

```csharp
public class GetMessageDataTransformConfiguration<TInput, TValue> : IMessageDataTransformConfiguration<TInput>
```

#### Type Parameters

`TInput`<br/>

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [GetMessageDataTransformConfiguration\<TInput, TValue\>](../masstransit-messagedata-configuration/getmessagedatatransformconfiguration-2)<br/>
Implements [IMessageDataTransformConfiguration\<TInput\>](../masstransit-messagedata-configuration/imessagedatatransformconfiguration-1)

## Constructors

### **GetMessageDataTransformConfiguration(IMessageDataRepository, PropertyInfo)**

```csharp
public GetMessageDataTransformConfiguration(IMessageDataRepository repository, PropertyInfo property)
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

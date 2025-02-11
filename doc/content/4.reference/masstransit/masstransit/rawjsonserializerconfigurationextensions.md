---

title: RawJsonSerializerConfigurationExtensions

---

# RawJsonSerializerConfigurationExtensions

Namespace: MassTransit

```csharp
public static class RawJsonSerializerConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RawJsonSerializerConfigurationExtensions](../masstransit/rawjsonserializerconfigurationextensions)

## Methods

### **UseRawJsonSerializer(IBusFactoryConfigurator, RawSerializerOptions, Boolean)**

Serialize and deserialize messages using the raw JSON message serializer

```csharp
public static void UseRawJsonSerializer(IBusFactoryConfigurator configurator, RawSerializerOptions options, bool isDefault)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`options` [RawSerializerOptions](../masstransit/rawserializeroptions)<br/>
Options for the raw serializer behavior

`isDefault` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
If true, set the default content type to the content type of the deserializer

### **AddRawJsonSerializer(IBusFactoryConfigurator, RawSerializerOptions)**

Add support for RAW JSON message serialization and deserialization (does not change the default serializer)

```csharp
public static void AddRawJsonSerializer(IBusFactoryConfigurator configurator, RawSerializerOptions options)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`options` [RawSerializerOptions](../masstransit/rawserializeroptions)<br/>
Options for the raw serializer behavior

### **UseRawJsonDeserializer(IBusFactoryConfigurator, RawSerializerOptions, Boolean)**

Deserialize messages using the raw JSON message serializer

```csharp
public static void UseRawJsonDeserializer(IBusFactoryConfigurator configurator, RawSerializerOptions options, bool isDefault)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`options` [RawSerializerOptions](../masstransit/rawserializeroptions)<br/>
Options for the raw serializer behavior

`isDefault` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
If true, set the default content type to the content type of the deserializer

### **UseRawJsonSerializer(IReceiveEndpointConfigurator, RawSerializerOptions, Boolean)**

Serialize and deserialize messages using the raw JSON message serializer

```csharp
public static void UseRawJsonSerializer(IReceiveEndpointConfigurator configurator, RawSerializerOptions options, bool isDefault)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`options` [RawSerializerOptions](../masstransit/rawserializeroptions)<br/>
Options for the raw serializer behavior

`isDefault` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
If true, set the default content type to the content type of the deserializer

### **UseRawJsonDeserializer(IReceiveEndpointConfigurator, RawSerializerOptions, Boolean)**

Deserialize messages using the raw JSON message serializer

```csharp
public static void UseRawJsonDeserializer(IReceiveEndpointConfigurator configurator, RawSerializerOptions options, bool isDefault)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`options` [RawSerializerOptions](../masstransit/rawserializeroptions)<br/>
Options for the raw serializer behavior

`isDefault` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
If true, set the default content type to the content type of the deserializer

---

title: JsonSerializerConfigurationExtensions

---

# JsonSerializerConfigurationExtensions

Namespace: MassTransit

```csharp
public static class JsonSerializerConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JsonSerializerConfigurationExtensions](../masstransit/jsonserializerconfigurationextensions)

## Methods

### **UseJsonSerializer(IBusFactoryConfigurator)**

Serialize and deserialize messages using the raw JSON message serializer

```csharp
public static void UseJsonSerializer(IBusFactoryConfigurator configurator)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

### **UseJsonDeserializer(IBusFactoryConfigurator, Boolean)**

Deserialize messages using the raw JSON message serializer

```csharp
public static void UseJsonDeserializer(IBusFactoryConfigurator configurator, bool isDefault)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`isDefault` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
If true, set the default content type to the content type of the deserializer

### **UseJsonSerializer(IReceiveEndpointConfigurator)**

Serialize and deserialize messages using the raw JSON message serializer

```csharp
public static void UseJsonSerializer(IReceiveEndpointConfigurator configurator)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

### **UseJsonDeserializer(IReceiveEndpointConfigurator, Boolean)**

Deserialize messages using the raw JSON message serializer

```csharp
public static void UseJsonDeserializer(IReceiveEndpointConfigurator configurator, bool isDefault)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`isDefault` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
If true, set the default content type to the content type of the deserializer

### **ConfigureJsonSerializerOptions(IBusFactoryConfigurator, Func\<JsonSerializerOptions, JsonSerializerOptions\>)**

Configure the global shared options for the default System.Text.Json serializer

```csharp
public static void ConfigureJsonSerializerOptions(IBusFactoryConfigurator configurator, Func<JsonSerializerOptions, JsonSerializerOptions> configure)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`configure` [Func\<JsonSerializerOptions, JsonSerializerOptions\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **SetMessageSerializerOptions\<T\>(JsonSerializerOptions, Func\<JsonSerializerOptions, JsonSerializerOptions\>)**

Specify custom  for a message type, removing any previous configured options for the same message type.

```csharp
public static void SetMessageSerializerOptions<T>(JsonSerializerOptions options, Func<JsonSerializerOptions, JsonSerializerOptions> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`options` JsonSerializerOptions<br/>

`configure` [Func\<JsonSerializerOptions, JsonSerializerOptions\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

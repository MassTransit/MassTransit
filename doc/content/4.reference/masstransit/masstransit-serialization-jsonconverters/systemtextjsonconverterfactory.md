---

title: SystemTextJsonConverterFactory

---

# SystemTextJsonConverterFactory

Namespace: MassTransit.Serialization.JsonConverters

```csharp
public class SystemTextJsonConverterFactory : JsonConverterFactory
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → JsonConverter → JsonConverterFactory → [SystemTextJsonConverterFactory](../masstransit-serialization-jsonconverters/systemtextjsonconverterfactory)

## Properties

### **Type**

```csharp
public Type Type { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

## Constructors

### **SystemTextJsonConverterFactory()**

```csharp
public SystemTextJsonConverterFactory()
```

## Methods

### **CanConvert(Type)**

```csharp
public bool CanConvert(Type typeToConvert)
```

#### Parameters

`typeToConvert` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **CreateConverter(Type, JsonSerializerOptions)**

```csharp
public JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
```

#### Parameters

`typeToConvert` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`options` JsonSerializerOptions<br/>

#### Returns

JsonConverter<br/>

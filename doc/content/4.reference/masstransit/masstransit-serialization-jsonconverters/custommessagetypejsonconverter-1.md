---

title: CustomMessageTypeJsonConverter<T>

---

# CustomMessageTypeJsonConverter\<T\>

Namespace: MassTransit.Serialization.JsonConverters

```csharp
public class CustomMessageTypeJsonConverter<T> : JsonConverter<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → JsonConverter → JsonConverter\<T\> → [CustomMessageTypeJsonConverter\<T\>](../masstransit-serialization-jsonconverters/custommessagetypejsonconverter-1)

## Properties

### **HandleNull**

```csharp
public bool HandleNull { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Type**

```csharp
public Type Type { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

## Constructors

### **CustomMessageTypeJsonConverter(JsonSerializerOptions)**

```csharp
public CustomMessageTypeJsonConverter(JsonSerializerOptions options)
```

#### Parameters

`options` JsonSerializerOptions<br/>

## Methods

### **Read(Utf8JsonReader, Type, JsonSerializerOptions)**

```csharp
public T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
```

#### Parameters

`reader` Utf8JsonReader<br/>

`typeToConvert` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`options` JsonSerializerOptions<br/>

#### Returns

T<br/>

### **Write(Utf8JsonWriter, T, JsonSerializerOptions)**

```csharp
public void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
```

#### Parameters

`writer` Utf8JsonWriter<br/>

`value` T<br/>

`options` JsonSerializerOptions<br/>

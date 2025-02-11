---

title: TypeMappingJsonConverter<TType, TImplementation>

---

# TypeMappingJsonConverter\<TType, TImplementation\>

Namespace: MassTransit.Serialization.JsonConverters

```csharp
public class TypeMappingJsonConverter<TType, TImplementation> : JsonConverter<TType>
```

#### Type Parameters

`TType`<br/>

`TImplementation`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → JsonConverter → JsonConverter\<TType\> → [TypeMappingJsonConverter\<TType, TImplementation\>](../masstransit-serialization-jsonconverters/typemappingjsonconverter-2)

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

### **TypeMappingJsonConverter()**

```csharp
public TypeMappingJsonConverter()
```

## Methods

### **Read(Utf8JsonReader, Type, JsonSerializerOptions)**

```csharp
public TType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
```

#### Parameters

`reader` Utf8JsonReader<br/>

`typeToConvert` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`options` JsonSerializerOptions<br/>

#### Returns

TType<br/>

### **Write(Utf8JsonWriter, TType, JsonSerializerOptions)**

```csharp
public void Write(Utf8JsonWriter writer, TType value, JsonSerializerOptions options)
```

#### Parameters

`writer` Utf8JsonWriter<br/>

`value` TType<br/>

`options` JsonSerializerOptions<br/>

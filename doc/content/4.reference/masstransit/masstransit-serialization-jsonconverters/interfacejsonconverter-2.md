---

title: InterfaceJsonConverter<TType, TImplementation>

---

# InterfaceJsonConverter\<TType, TImplementation\>

Namespace: MassTransit.Serialization.JsonConverters

```csharp
public class InterfaceJsonConverter<TType, TImplementation> : JsonConverter<TType>
```

#### Type Parameters

`TType`<br/>

`TImplementation`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → JsonConverter → JsonConverter\<TType\> → [InterfaceJsonConverter\<TType, TImplementation\>](../masstransit-serialization-jsonconverters/interfacejsonconverter-2)

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

### **InterfaceJsonConverter()**

```csharp
public InterfaceJsonConverter()
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

---

title: StringDecimalJsonConverter

---

# StringDecimalJsonConverter

Namespace: MassTransit.Serialization.JsonConverters

```csharp
public class StringDecimalJsonConverter : JsonConverter<Decimal>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → JsonConverter → JsonConverter\<Decimal\> → [StringDecimalJsonConverter](../masstransit-serialization-jsonconverters/stringdecimaljsonconverter)

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

### **StringDecimalJsonConverter()**

```csharp
public StringDecimalJsonConverter()
```

## Methods

### **Read(Utf8JsonReader, Type, JsonSerializerOptions)**

```csharp
public decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
```

#### Parameters

`reader` Utf8JsonReader<br/>

`typeToConvert` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`options` JsonSerializerOptions<br/>

#### Returns

[Decimal](https://learn.microsoft.com/en-us/dotnet/api/system.decimal)<br/>

### **Write(Utf8JsonWriter, Decimal, JsonSerializerOptions)**

```csharp
public void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
```

#### Parameters

`writer` Utf8JsonWriter<br/>

`value` [Decimal](https://learn.microsoft.com/en-us/dotnet/api/system.decimal)<br/>

`options` JsonSerializerOptions<br/>

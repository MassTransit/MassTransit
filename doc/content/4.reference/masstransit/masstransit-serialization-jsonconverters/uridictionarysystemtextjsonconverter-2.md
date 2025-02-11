---

title: UriDictionarySystemTextJsonConverter<T, TValue>

---

# UriDictionarySystemTextJsonConverter\<T, TValue\>

Namespace: MassTransit.Serialization.JsonConverters

```csharp
public class UriDictionarySystemTextJsonConverter<T, TValue> : JsonConverter<T>
```

#### Type Parameters

`T`<br/>

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → JsonConverter → JsonConverter\<T\> → [UriDictionarySystemTextJsonConverter\<T, TValue\>](../masstransit-serialization-jsonconverters/uridictionarysystemtextjsonconverter-2)

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

### **UriDictionarySystemTextJsonConverter()**

```csharp
public UriDictionarySystemTextJsonConverter()
```

## Methods

### **Write(Utf8JsonWriter, T, JsonSerializerOptions)**

```csharp
public void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
```

#### Parameters

`writer` Utf8JsonWriter<br/>

`value` T<br/>

`options` JsonSerializerOptions<br/>

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

### **ReadInternal(Utf8JsonReader, Type, JsonSerializerOptions)**

```csharp
protected T ReadInternal(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
```

#### Parameters

`reader` Utf8JsonReader<br/>

`typeToConvert` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`options` JsonSerializerOptions<br/>

#### Returns

T<br/>

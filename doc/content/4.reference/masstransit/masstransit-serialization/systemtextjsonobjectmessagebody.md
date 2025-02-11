---

title: SystemTextJsonObjectMessageBody

---

# SystemTextJsonObjectMessageBody

Namespace: MassTransit.Serialization

```csharp
public class SystemTextJsonObjectMessageBody : MessageBody
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SystemTextJsonObjectMessageBody](../masstransit-serialization/systemtextjsonobjectmessagebody)<br/>
Implements [MessageBody](../../masstransit-abstractions/masstransit/messagebody)

## Properties

### **Length**

```csharp
public Nullable<long> Length { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **SystemTextJsonObjectMessageBody(Object, JsonSerializerOptions)**

```csharp
public SystemTextJsonObjectMessageBody(object value, JsonSerializerOptions options)
```

#### Parameters

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`options` JsonSerializerOptions<br/>

## Methods

### **GetStream()**

```csharp
public Stream GetStream()
```

#### Returns

[Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

### **GetBytes()**

```csharp
public Byte[] GetBytes()
```

#### Returns

[Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

### **GetString()**

```csharp
public string GetString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

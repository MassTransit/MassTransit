---

title: SystemTextJsonRawMessageBody<TMessage>

---

# SystemTextJsonRawMessageBody\<TMessage\>

Namespace: MassTransit.Serialization

```csharp
public class SystemTextJsonRawMessageBody<TMessage> : MessageBody
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SystemTextJsonRawMessageBody\<TMessage\>](../masstransit-serialization/systemtextjsonrawmessagebody-1)<br/>
Implements [MessageBody](../../masstransit-abstractions/masstransit/messagebody)

## Properties

### **Length**

```csharp
public Nullable<long> Length { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **SystemTextJsonRawMessageBody(SendContext\<TMessage\>, JsonSerializerOptions, Object)**

```csharp
public SystemTextJsonRawMessageBody(SendContext<TMessage> context, JsonSerializerOptions options, object message)
```

#### Parameters

`context` [SendContext\<TMessage\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

`options` JsonSerializerOptions<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

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

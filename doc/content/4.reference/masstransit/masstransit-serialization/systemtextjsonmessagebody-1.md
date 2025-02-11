---

title: SystemTextJsonMessageBody<TMessage>

---

# SystemTextJsonMessageBody\<TMessage\>

Namespace: MassTransit.Serialization

```csharp
public class SystemTextJsonMessageBody<TMessage> : MessageBody
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SystemTextJsonMessageBody\<TMessage\>](../masstransit-serialization/systemtextjsonmessagebody-1)<br/>
Implements [MessageBody](../../masstransit-abstractions/masstransit/messagebody)

## Properties

### **Length**

```csharp
public Nullable<long> Length { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **SystemTextJsonMessageBody(SendContext\<TMessage\>, JsonSerializerOptions, MessageEnvelope)**

```csharp
public SystemTextJsonMessageBody(SendContext<TMessage> context, JsonSerializerOptions options, MessageEnvelope envelope)
```

#### Parameters

`context` [SendContext\<TMessage\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

`options` JsonSerializerOptions<br/>

`envelope` [MessageEnvelope](../../masstransit-abstractions/masstransit-serialization/messageenvelope)<br/>

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

---

title: NotSupportedMessageBody

---

# NotSupportedMessageBody

Namespace: MassTransit.Serialization

```csharp
public class NotSupportedMessageBody : MessageBody
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NotSupportedMessageBody](../masstransit-serialization/notsupportedmessagebody)<br/>
Implements [MessageBody](../../masstransit-abstractions/masstransit/messagebody)

## Properties

### **Length**

```csharp
public Nullable<long> Length { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **NotSupportedMessageBody()**

```csharp
public NotSupportedMessageBody()
```

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

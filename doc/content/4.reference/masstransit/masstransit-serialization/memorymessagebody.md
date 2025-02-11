---

title: MemoryMessageBody

---

# MemoryMessageBody

Namespace: MassTransit.Serialization

```csharp
public class MemoryMessageBody : MessageBody
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MemoryMessageBody](../masstransit-serialization/memorymessagebody)<br/>
Implements [MessageBody](../../masstransit-abstractions/masstransit/messagebody)

## Properties

### **Length**

```csharp
public Nullable<long> Length { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **MemoryMessageBody(ReadOnlyMemory\<Byte\>)**

```csharp
public MemoryMessageBody(ReadOnlyMemory<byte> memory)
```

#### Parameters

`memory` [ReadOnlyMemory\<Byte\>](https://learn.microsoft.com/en-us/dotnet/api/system.readonlymemory-1)<br/>

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

---

title: ArrayMessageBody

---

# ArrayMessageBody

Namespace: MassTransit

```csharp
public class ArrayMessageBody : MessageBody
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ArrayMessageBody](../masstransit/arraymessagebody)<br/>
Implements [MessageBody](../masstransit/messagebody)

## Properties

### **Length**

```csharp
public Nullable<long> Length { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **ArrayMessageBody(ArraySegment\<Byte\>)**

```csharp
public ArrayMessageBody(ArraySegment<byte> bytes)
```

#### Parameters

`bytes` [ArraySegment\<Byte\>](https://learn.microsoft.com/en-us/dotnet/api/system.arraysegment-1)<br/>

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

---

title: StringMessageBody

---

# StringMessageBody

Namespace: MassTransit

```csharp
public class StringMessageBody : MessageBody
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StringMessageBody](../masstransit/stringmessagebody)<br/>
Implements [MessageBody](../masstransit/messagebody)

## Properties

### **Length**

```csharp
public Nullable<long> Length { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **StringMessageBody(String)**

```csharp
public StringMessageBody(string body)
```

#### Parameters

`body` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

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

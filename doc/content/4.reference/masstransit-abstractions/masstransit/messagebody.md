---

title: MessageBody

---

# MessageBody

Namespace: MassTransit

```csharp
public interface MessageBody
```

## Properties

### **Length**

```csharp
public abstract Nullable<long> Length { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Methods

### **GetStream()**

Return the message body as a stream

```csharp
Stream GetStream()
```

#### Returns

[Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

### **GetBytes()**

Return the message body as a byte array

```csharp
Byte[] GetBytes()
```

#### Returns

[Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

### **GetString()**

Return the message body as a string

```csharp
string GetString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

---

title: EmptyMessageBody

---

# EmptyMessageBody

Namespace: MassTransit

```csharp
public class EmptyMessageBody : MessageBody
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EmptyMessageBody](../masstransit/emptymessagebody)<br/>
Implements [MessageBody](../masstransit/messagebody)

## Properties

### **Instance**

```csharp
public static MessageBody Instance { get; }
```

#### Property Value

[MessageBody](../masstransit/messagebody)<br/>

### **Length**

```csharp
public Nullable<long> Length { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **EmptyMessageBody()**

```csharp
public EmptyMessageBody()
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

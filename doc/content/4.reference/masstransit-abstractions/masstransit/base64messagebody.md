---

title: Base64MessageBody

---

# Base64MessageBody

Namespace: MassTransit

Converts a binary-only message body to Base64 so that it can be used with non-binary message transports.
 Only the [Base64MessageBody.GetString()](base64messagebody#getstring) method performs the conversion, [Base64MessageBody.GetBytes()](base64messagebody#getbytes) and [Base64MessageBody.GetStream()](base64messagebody#getstream)
 are pass-through methods.

```csharp
public class Base64MessageBody : MessageBody
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Base64MessageBody](../masstransit/base64messagebody)<br/>
Implements [MessageBody](../masstransit/messagebody)

## Properties

### **Length**

```csharp
public Nullable<long> Length { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **Base64MessageBody(String)**

```csharp
public Base64MessageBody(string text)
```

#### Parameters

`text` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

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

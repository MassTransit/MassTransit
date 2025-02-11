---

title: MessageUrn

---

# MessageUrn

Namespace: MassTransit

```csharp
public class MessageUrn : Uri, ISpanFormattable, IFormattable, ISerializable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → Uri → [MessageUrn](../masstransit/messageurn)<br/>
Implements [ISpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable), [ISerializable](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.iserializable)

## Fields

### **Prefix**

```csharp
public static string Prefix;
```

## Properties

### **AbsolutePath**

```csharp
public string AbsolutePath { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **AbsoluteUri**

```csharp
public string AbsoluteUri { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **LocalPath**

```csharp
public string LocalPath { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Authority**

```csharp
public string Authority { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **HostNameType**

```csharp
public UriHostNameType HostNameType { get; }
```

#### Property Value

UriHostNameType<br/>

### **IsDefaultPort**

```csharp
public bool IsDefaultPort { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsFile**

```csharp
public bool IsFile { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsLoopback**

```csharp
public bool IsLoopback { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PathAndQuery**

```csharp
public string PathAndQuery { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Segments**

```csharp
public String[] Segments { get; }
```

#### Property Value

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IsUnc**

```csharp
public bool IsUnc { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Host**

```csharp
public string Host { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Port**

```csharp
public int Port { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Query**

```csharp
public string Query { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Fragment**

```csharp
public string Fragment { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Scheme**

```csharp
public string Scheme { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **OriginalString**

```csharp
public string OriginalString { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **DnsSafeHost**

```csharp
public string DnsSafeHost { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IdnHost**

```csharp
public string IdnHost { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IsAbsoluteUri**

```csharp
public bool IsAbsoluteUri { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **UserEscaped**

```csharp
public bool UserEscaped { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **UserInfo**

```csharp
public string UserInfo { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **ForType\<T\>()**

```csharp
public static MessageUrn ForType<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[MessageUrn](../masstransit/messageurn)<br/>

### **ForTypeString\<T\>()**

```csharp
public static string ForTypeString<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ForType(Type)**

```csharp
public static MessageUrn ForType(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[MessageUrn](../masstransit/messageurn)<br/>

### **ForTypeString(Type)**

```csharp
public static string ForTypeString(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Deconstruct(String, String, String)**

```csharp
public void Deconstruct(out string name, out string ns, out string assemblyName)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`ns` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`assemblyName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

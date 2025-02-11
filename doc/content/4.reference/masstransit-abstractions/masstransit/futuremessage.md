---

title: FutureMessage

---

# FutureMessage

Namespace: MassTransit

```csharp
public class FutureMessage
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureMessage](../masstransit/futuremessage)

## Properties

### **Message**

```csharp
public IDictionary<string, object> Message { get; set; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

### **SupportedMessageTypes**

```csharp
public String[] SupportedMessageTypes { get; set; }
```

#### Property Value

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **FutureMessage()**

```csharp
public FutureMessage()
```

### **FutureMessage(IDictionary\<String, Object\>, String[])**

```csharp
public FutureMessage(IDictionary<string, object> message, String[] supportedMessageTypes)
```

#### Parameters

`message` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`supportedMessageTypes` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **HasMessageType(Type)**

```csharp
public bool HasMessageType(Type messageType)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **HasMessageType\<T\>()**

```csharp
public bool HasMessageType<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

---

title: ReceiveContextExtensions

---

# ReceiveContextExtensions

Namespace: MassTransit

```csharp
public static class ReceiveContextExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveContextExtensions](../masstransit/receivecontextextensions)

## Methods

### **GetMessageId(ReceiveContext)**

Returns the messageId from the transport header, if available

```csharp
public static Nullable<Guid> GetMessageId(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>

#### Returns

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetMessageId(ReceiveContext, Guid)**

Returns the messageId from the transport header, if available

```csharp
public static Guid GetMessageId(ReceiveContext context, Guid defaultValue)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>

`defaultValue` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **GetCorrelationId(ReceiveContext)**

Returns the CorrelationId from the transport header, if available

```csharp
public static Nullable<Guid> GetCorrelationId(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>

#### Returns

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetConversationId(ReceiveContext)**

Returns the ConversationId from the transport header, if available

```csharp
public static Nullable<Guid> GetConversationId(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>

#### Returns

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetRequestId(ReceiveContext)**

Returns the RequestId from the transport header, if available

```csharp
public static Nullable<Guid> GetRequestId(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>

#### Returns

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetInitiatorId(ReceiveContext)**

Returns the InitiatorId from the transport header, if available

```csharp
public static Nullable<Guid> GetInitiatorId(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>

#### Returns

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetSourceAddress(ReceiveContext)**

Returns the SourceAddress from the transport headers, if present

```csharp
public static Uri GetSourceAddress(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>

#### Returns

Uri<br/>

### **GetResponseAddress(ReceiveContext)**

Returns the ResponseAddress from the transport headers, if present

```csharp
public static Uri GetResponseAddress(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>

#### Returns

Uri<br/>

### **GetFaultAddress(ReceiveContext)**

Returns the FaultAddress from the transport headers, if present

```csharp
public static Uri GetFaultAddress(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>

#### Returns

Uri<br/>

### **GetSentTime(ReceiveContext)**

Returns the message sent timestamp from the transport (not message headers)

```csharp
public static Nullable<DateTime> GetSentTime(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>

#### Returns

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetMessageTypes(ReceiveContext)**

```csharp
public static String[] GetMessageTypes(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>

#### Returns

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **GetMessageEncoding(ReceiveContext)**

Returns either the Content-Encoding from the transport header, or the default UTF-8 encoding (no BOM).

```csharp
public static Encoding GetMessageEncoding(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>

#### Returns

[Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding)<br/>

### **GetMessageId(Headers)**

Returns the messageId from the transport header, if available

```csharp
public static Nullable<Guid> GetMessageId(Headers headers)
```

#### Parameters

`headers` [Headers](../masstransit/headers)<br/>

#### Returns

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetMessageId(Headers, Guid)**

Returns the messageId from the transport header, if available

```csharp
public static Guid GetMessageId(Headers headers, Guid defaultValue)
```

#### Parameters

`headers` [Headers](../masstransit/headers)<br/>

`defaultValue` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **GetCorrelationId(Headers)**

Returns the CorrelationId from the transport header, if available

```csharp
public static Nullable<Guid> GetCorrelationId(Headers headers)
```

#### Parameters

`headers` [Headers](../masstransit/headers)<br/>

#### Returns

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetConversationId(Headers)**

Returns the ConversationId from the transport header, if available

```csharp
public static Nullable<Guid> GetConversationId(Headers headers)
```

#### Parameters

`headers` [Headers](../masstransit/headers)<br/>

#### Returns

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetRequestId(Headers)**

Returns the RequestId from the transport header, if available

```csharp
public static Nullable<Guid> GetRequestId(Headers headers)
```

#### Parameters

`headers` [Headers](../masstransit/headers)<br/>

#### Returns

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetInitiatorId(Headers)**

Returns the InitiatorId from the transport header, if available

```csharp
public static Nullable<Guid> GetInitiatorId(Headers headers)
```

#### Parameters

`headers` [Headers](../masstransit/headers)<br/>

#### Returns

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetSourceAddress(Headers)**

Returns the SourceAddress from the transport headers, if present

```csharp
public static Uri GetSourceAddress(Headers headers)
```

#### Parameters

`headers` [Headers](../masstransit/headers)<br/>

#### Returns

Uri<br/>

### **GetResponseAddress(Headers)**

Returns the ResponseAddress from the transport headers, if present

```csharp
public static Uri GetResponseAddress(Headers headers)
```

#### Parameters

`headers` [Headers](../masstransit/headers)<br/>

#### Returns

Uri<br/>

### **GetFaultAddress(Headers)**

Returns the FaultAddress from the transport headers, if present

```csharp
public static Uri GetFaultAddress(Headers headers)
```

#### Parameters

`headers` [Headers](../masstransit/headers)<br/>

#### Returns

Uri<br/>

### **GetMessageTypes(Headers)**

```csharp
public static String[] GetMessageTypes(Headers headers)
```

#### Parameters

`headers` [Headers](../masstransit/headers)<br/>

#### Returns

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **GetMessageEncoding(Headers)**

Returns either the Content-Encoding from the transport header, or the default UTF-8 encoding (no BOM).

```csharp
public static Encoding GetMessageEncoding(Headers headers)
```

#### Parameters

`headers` [Headers](../masstransit/headers)<br/>

#### Returns

[Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding)<br/>

### **GetHeaderId(Headers, String, Guid)**

```csharp
public static Guid GetHeaderId(Headers headers, string key, Guid defaultValue)
```

#### Parameters

`headers` [Headers](../masstransit/headers)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`defaultValue` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **GetHeaderId(Headers, String)**

```csharp
public static Nullable<Guid> GetHeaderId(Headers headers, string key)
```

#### Parameters

`headers` [Headers](../masstransit/headers)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetEndpointAddress(Headers, String)**

```csharp
public static Uri GetEndpointAddress(Headers headers, string key)
```

#### Parameters

`headers` [Headers](../masstransit/headers)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

Uri<br/>

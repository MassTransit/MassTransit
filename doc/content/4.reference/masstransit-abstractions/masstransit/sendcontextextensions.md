---

title: SendContextExtensions

---

# SendContextExtensions

Namespace: MassTransit

```csharp
public static class SendContextExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendContextExtensions](../masstransit/sendcontextextensions)

## Methods

### **SetHostHeaders(SendHeaders)**

Set the host headers on the SendContext (for error, dead-letter, etc.)

```csharp
public static void SetHostHeaders(SendHeaders headers)
```

#### Parameters

`headers` [SendHeaders](../masstransit/sendheaders)<br/>

### **SetHostHeaders\<T\>(ITransportSetHeaderAdapter\<T\>, IDictionary\<String, T\>)**

Set the host headers on the SendContext (for error, dead-letter, etc.)

```csharp
public static void SetHostHeaders<T>(ITransportSetHeaderAdapter<T> adapter, IDictionary<string, T> dictionary)
```

#### Type Parameters

`T`<br/>

#### Parameters

`adapter` [ITransportSetHeaderAdapter\<T\>](../masstransit-transports/itransportsetheaderadapter-1)<br/>

`dictionary` [IDictionary\<String, T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

### **SetExceptionHeaders(SendHeaders, ExceptionReceiveContext)**

Set the host headers on the SendContext (for error, dead-letter, etc.)

```csharp
public static void SetExceptionHeaders(SendHeaders headers, ExceptionReceiveContext exceptionContext)
```

#### Parameters

`headers` [SendHeaders](../masstransit/sendheaders)<br/>

`exceptionContext` [ExceptionReceiveContext](../masstransit/exceptionreceivecontext)<br/>

### **SetExceptionHeaders\<T\>(ITransportSetHeaderAdapter\<T\>, IDictionary\<String, T\>, ExceptionReceiveContext)**

Set the host headers on the SendContext (for error, dead-letter, etc.)

```csharp
public static void SetExceptionHeaders<T>(ITransportSetHeaderAdapter<T> adapter, IDictionary<string, T> headers, ExceptionReceiveContext exceptionContext)
```

#### Type Parameters

`T`<br/>

#### Parameters

`adapter` [ITransportSetHeaderAdapter\<T\>](../masstransit-transports/itransportsetheaderadapter-1)<br/>

`headers` [IDictionary\<String, T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`exceptionContext` [ExceptionReceiveContext](../masstransit/exceptionreceivecontext)<br/>

### **TransferConsumeContextHeaders(SendContext, ConsumeContext)**

Transfer the header information from the ConsumeContext to the SendContext, including any non-MT headers.

```csharp
public static void TransferConsumeContextHeaders(SendContext sendContext, ConsumeContext consumeContext)
```

#### Parameters

`sendContext` [SendContext](../masstransit/sendcontext)<br/>

`consumeContext` [ConsumeContext](../masstransit/consumecontext)<br/>

### **ApplyRedeliveryOptions(SendContext, ConsumeContext, RedeliveryOptions)**

```csharp
public static void ApplyRedeliveryOptions(SendContext sendContext, ConsumeContext consumeContext, RedeliveryOptions options)
```

#### Parameters

`sendContext` [SendContext](../masstransit/sendcontext)<br/>

`consumeContext` [ConsumeContext](../masstransit/consumecontext)<br/>

`options` [RedeliveryOptions](../masstransit/redeliveryoptions)<br/>

### **ReplaceMessageId(SendContext, ConsumeContext)**

Generate a new MessageId, storing the original MessageId in the OriginalMessageId header (unless it already exists)

```csharp
public static SendContext ReplaceMessageId(SendContext sendContext, ConsumeContext consumeContext)
```

#### Parameters

`sendContext` [SendContext](../masstransit/sendcontext)<br/>

`consumeContext` [ConsumeContext](../masstransit/consumecontext)<br/>

#### Returns

[SendContext](../masstransit/sendcontext)<br/>

### **GetOriginalMessageId(ConsumeContext)**

Returns the original MessageId from the message headers, or the MessageId if not present

```csharp
public static Nullable<Guid> GetOriginalMessageId(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

#### Returns

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartNewConversation(SendContext)**

Sets the ConversationId to a new value, starting a new conversation. If a message was being consumed, and the
 ConversationId was present, that value is stored in an MT-InitiatingConversationId header.

```csharp
public static SendContext StartNewConversation(SendContext context)
```

#### Parameters

`context` [SendContext](../masstransit/sendcontext)<br/>
The send context

#### Returns

[SendContext](../masstransit/sendcontext)<br/>

### **StartNewConversation(SendContext, Guid)**

Sets the ConversationId to a new value, starting a new conversation. If a message was being consumed, and the
 ConversationId was present, that value is stored in an MT-InitiatingConversationId header.

```csharp
public static SendContext StartNewConversation(SendContext context, Guid conversationId)
```

#### Parameters

`context` [SendContext](../masstransit/sendcontext)<br/>
The send context

`conversationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
The new ConversationId

#### Returns

[SendContext](../masstransit/sendcontext)<br/>

---

title: MessageResponse<TResult>

---

# MessageResponse\<TResult\>

Namespace: MassTransit.Clients

A result from a request

```csharp
public class MessageResponse<TResult> : Response<TResult>, Response, MessageContext
```

#### Type Parameters

`TResult`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageResponse\<TResult\>](../masstransit-clients/messageresponse-1)<br/>
Implements [Response\<TResult\>](../../masstransit-abstractions/masstransit/response-1), [Response](../../masstransit-abstractions/masstransit/response), [MessageContext](../../masstransit-abstractions/masstransit/messagecontext)

## Properties

### **Message**

```csharp
public TResult Message { get; }
```

#### Property Value

TResult<br/>

## Constructors

### **MessageResponse(ConsumeContext\<TResult\>)**

```csharp
public MessageResponse(ConsumeContext<TResult> context)
```

#### Parameters

`context` [ConsumeContext\<TResult\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

## Methods

### **DeserializeObject\<T\>(Dictionary\<String, Object\>)**

```csharp
public T DeserializeObject<T>(Dictionary<string, object> dictionary)
```

#### Type Parameters

`T`<br/>

#### Parameters

`dictionary` [Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

#### Returns

T<br/>

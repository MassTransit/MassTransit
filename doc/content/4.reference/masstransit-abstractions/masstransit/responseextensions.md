---

title: ResponseExtensions

---

# ResponseExtensions

Namespace: MassTransit

```csharp
public static class ResponseExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ResponseExtensions](../masstransit/responseextensions)

## Methods

### **Deconstruct(Response, Response, Object)**

Used for pattern matching via (response,message)

```csharp
public static void Deconstruct(Response response, out Response context, out object message)
```

#### Parameters

`response` [Response](../masstransit/response)<br/>

`context` [Response](../masstransit/response)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **IsResponseAccepted\<T\>(ConsumeContext, Boolean)**

Returns true if the response type is explicitly accepted, or if the accept response header is
 not present (downlevel client).

```csharp
public static bool IsResponseAccepted<T>(ConsumeContext context, bool defaultIfHeaderNotFound)
```

#### Type Parameters

`T`<br/>
The response type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>
The consumed message context

`defaultIfHeaderNotFound` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
Value to return if header was not present

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
True if explicitly support or header is missing, otherwise false

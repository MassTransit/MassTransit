---

title: Response<TResponse>

---

# Response\<TResponse\>

Namespace: MassTransit

The response for a request with a single response type, or a request with multiple response types
 that has been matched to a specific type.

```csharp
public interface Response<TResponse> : Response, MessageContext
```

#### Type Parameters

`TResponse`<br/>
The response type

Implements [Response](../masstransit/response), [MessageContext](../masstransit/messagecontext)

## Properties

### **Message**

The response message that was received

```csharp
public abstract TResponse Message { get; }
```

#### Property Value

TResponse<br/>

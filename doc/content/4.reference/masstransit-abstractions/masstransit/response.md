---

title: Response

---

# Response

Namespace: MassTransit

The base response type, which can be used to pattern match, via deconstruct, to the accepted
 response types.

```csharp
public interface Response : MessageContext
```

Implements [MessageContext](../masstransit/messagecontext)

## Properties

### **Message**

```csharp
public abstract object Message { get; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

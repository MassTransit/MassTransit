---

title: ReceiveContextBodyExtensions

---

# ReceiveContextBodyExtensions

Namespace: MassTransit

```csharp
public static class ReceiveContextBodyExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveContextBodyExtensions](../masstransit/receivecontextbodyextensions)

## Methods

### **GetBodyStream(ReceiveContext)**

Returns the message body as a stream that can be deserialized. The stream
 must be disposed by the caller, a reference is not retained

```csharp
public static Stream GetBodyStream(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>

#### Returns

[Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

### **GetBody(ReceiveContext)**

Returns the body as a byte[]

```csharp
public static Byte[] GetBody(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>

#### Returns

[Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

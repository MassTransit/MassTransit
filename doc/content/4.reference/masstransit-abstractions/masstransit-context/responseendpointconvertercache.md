---

title: ResponseEndpointConverterCache

---

# ResponseEndpointConverterCache

Namespace: MassTransit.Context

Caches the converters that allow a raw object to be published using the object's type through
 the generic Send method.

```csharp
public class ResponseEndpointConverterCache
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ResponseEndpointConverterCache](../masstransit-context/responseendpointconvertercache)

## Constructors

### **ResponseEndpointConverterCache()**

```csharp
public ResponseEndpointConverterCache()
```

## Methods

### **Respond(ConsumeContext, Object, Type)**

```csharp
public static Task Respond(ConsumeContext consumeContext, object message, Type messageType)
```

#### Parameters

`consumeContext` [ConsumeContext](../masstransit/consumecontext)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Respond(ConsumeContext, Object, Type, IPipe\<SendContext\>)**

```csharp
public static Task Respond(ConsumeContext consumeContext, object message, Type messageType, IPipe<SendContext> pipe)
```

#### Parameters

`consumeContext` [ConsumeContext](../masstransit/consumecontext)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

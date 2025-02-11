---

title: SendEndpointConverterCache

---

# SendEndpointConverterCache

Namespace: MassTransit.Context

Caches the converters that allow a raw object to be published using the object's type through
 the generic Send method.

```csharp
public class SendEndpointConverterCache
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendEndpointConverterCache](../masstransit-context/sendendpointconvertercache)

## Constructors

### **SendEndpointConverterCache()**

```csharp
public SendEndpointConverterCache()
```

## Methods

### **Send(ISendEndpoint, Object, Type, CancellationToken)**

```csharp
public static Task Send(ISendEndpoint endpoint, object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send(ISendEndpoint, Object, Type, IPipe\<SendContext\>, CancellationToken)**

```csharp
public static Task Send(ISendEndpoint endpoint, object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendInitializer(ISendEndpoint, Type, Object, CancellationToken)**

```csharp
public static Task SendInitializer(ISendEndpoint endpoint, Type messageType, object values, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendInitializer(ISendEndpoint, Type, Object, IPipe\<SendContext\>, CancellationToken)**

```csharp
public static Task SendInitializer(ISendEndpoint endpoint, Type messageType, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

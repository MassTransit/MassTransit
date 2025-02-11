---

title: PublishEndpointConverterCache

---

# PublishEndpointConverterCache

Namespace: MassTransit.Context

Caches the converters that allow a raw object to be published using the object's type through
 the generic Send method.

```csharp
public class PublishEndpointConverterCache
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishEndpointConverterCache](../masstransit-context/publishendpointconvertercache)

## Constructors

### **PublishEndpointConverterCache()**

```csharp
public PublishEndpointConverterCache()
```

## Methods

### **Publish(IPublishEndpoint, Object, Type, CancellationToken)**

```csharp
public static Task Publish(IPublishEndpoint endpoint, object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish(IPublishEndpoint, Object, Type, IPipe\<PublishContext\>, CancellationToken)**

```csharp
public static Task Publish(IPublishEndpoint endpoint, object message, Type messageType, IPipe<PublishContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`pipe` [IPipe\<PublishContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PublishInitializer(IPublishEndpoint, Type, Object, CancellationToken)**

```csharp
public static Task PublishInitializer(IPublishEndpoint endpoint, Type messageType, object values, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PublishInitializer(IPublishEndpoint, Type, Object, IPipe\<PublishContext\>, CancellationToken)**

```csharp
public static Task PublishInitializer(IPublishEndpoint endpoint, Type messageType, object values, IPipe<PublishContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`endpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<PublishContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

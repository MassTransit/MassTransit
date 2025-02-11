---

title: PublishEndpointExtensions

---

# PublishEndpointExtensions

Namespace: MassTransit

```csharp
public static class PublishEndpointExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishEndpointExtensions](../masstransit/publishendpointextensions)

## Methods

### **Publish(IPublishEndpoint, Type, Object, CancellationToken)**

Publish a dynamically typed message initialized by a loosely typed dictionary of values. MassTransit will
 create and populate an object instance with the properties of the  argument.

```csharp
public static Task Publish(IPublishEndpoint publishEndpoint, Type messageType, object values, CancellationToken cancellationToken)
```

#### Parameters

`publishEndpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The message type to publish

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The dictionary of values to become hydrated and published under the type of the interface.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish(IPublishEndpoint, Type, Object, IPipe\<PublishContext\>, CancellationToken)**

Publish a dynamically typed message initialized by a loosely typed dictionary of values. MassTransit will
 create and populate an object instance with the properties of the  argument.

```csharp
public static Task Publish(IPublishEndpoint publishEndpoint, Type messageType, object values, IPipe<PublishContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`publishEndpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The message type to publish

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The dictionary of values to become hydrated and published under the type of the interface.

`pipe` [IPipe\<PublishContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

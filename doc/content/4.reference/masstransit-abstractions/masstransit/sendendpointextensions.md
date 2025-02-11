---

title: SendEndpointExtensions

---

# SendEndpointExtensions

Namespace: MassTransit

```csharp
public static class SendEndpointExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendEndpointExtensions](../masstransit/sendendpointextensions)

## Methods

### **Send(ISendEndpoint, Type, Object, CancellationToken)**

Send a dynamically typed message initialized by a loosely typed dictionary of values. MassTransit will
 create and populate an object instance with the properties of the  argument.

```csharp
public static Task Send(ISendEndpoint publishEndpoint, Type messageType, object values, CancellationToken cancellationToken)
```

#### Parameters

`publishEndpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The message type to publish

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The dictionary of values to become hydrated and published under the type of the interface.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send(ISendEndpoint, Type, Object, IPipe\<SendContext\>, CancellationToken)**

Send a dynamically typed message initialized by a loosely typed dictionary of values. MassTransit will
 create and populate an object instance with the properties of the  argument.

```csharp
public static Task Send(ISendEndpoint publishEndpoint, Type messageType, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`publishEndpoint` [ISendEndpoint](../masstransit/isendendpoint)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The message type to publish

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The dictionary of values to become hydrated and published under the type of the interface.

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

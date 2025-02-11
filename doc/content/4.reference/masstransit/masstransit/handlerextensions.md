---

title: HandlerExtensions

---

# HandlerExtensions

Namespace: MassTransit

```csharp
public static class HandlerExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [HandlerExtensions](../masstransit/handlerextensions)

## Methods

### **Handler\<T\>(IReceiveEndpointConfigurator, MessageHandler\<T\>, Action\<IHandlerConfigurator\<T\>\>)**

Adds a handler to the receive endpoint with additional configuration specified

```csharp
public static void Handler<T>(IReceiveEndpointConfigurator configurator, MessageHandler<T> handler, Action<IHandlerConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`handler` [MessageHandler\<T\>](../../masstransit-abstractions/masstransit/messagehandler-1)<br/>

`configure` [Action\<IHandlerConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConnectHandler\<T\>(IConsumePipeConnector, MessageHandler\<T\>, IBuildPipeConfigurator\<ConsumeContext\<T\>\>)**

Adds a message handler to the service bus for handling a specific type of message

```csharp
public static ConnectHandle ConnectHandler<T>(IConsumePipeConnector connector, MessageHandler<T> handler, IBuildPipeConfigurator<ConsumeContext<T>> configurator)
```

#### Type Parameters

`T`<br/>
The message type to handle, often inferred from the callback specified

#### Parameters

`connector` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`handler` [MessageHandler\<T\>](../../masstransit-abstractions/masstransit/messagehandler-1)<br/>
The callback to invoke when messages of the specified type arrive at the service bus

`configurator` [IBuildPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ibuildpipeconfigurator-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectRequestHandler\<T\>(IRequestPipeConnector, Guid, MessageHandler\<T\>, IBuildPipeConfigurator\<ConsumeContext\<T\>\>)**

Subscribe a request handler to the bus's endpoint

```csharp
public static ConnectHandle ConnectRequestHandler<T>(IRequestPipeConnector connector, Guid requestId, MessageHandler<T> handler, IBuildPipeConfigurator<ConsumeContext<T>> configurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`connector` [IRequestPipeConnector](../../masstransit-abstractions/masstransit/irequestpipeconnector)<br/>

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`handler` [MessageHandler\<T\>](../../masstransit-abstractions/masstransit/messagehandler-1)<br/>

`configurator` [IBuildPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ibuildpipeconfigurator-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

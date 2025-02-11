---

title: HandlerConnector<TMessage>

---

# HandlerConnector\<TMessage\>

Namespace: MassTransit.Configuration

Connects a message handler to a pipe

```csharp
public class HandlerConnector<TMessage> : IHandlerConnector<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [HandlerConnector\<TMessage\>](../masstransit-configuration/handlerconnector-1)<br/>
Implements [IHandlerConnector\<TMessage\>](../masstransit-configuration/ihandlerconnector-1)

## Constructors

### **HandlerConnector()**

```csharp
public HandlerConnector()
```

## Methods

### **ConnectHandler(IConsumePipeConnector, MessageHandler\<TMessage\>, IBuildPipeConfigurator\<ConsumeContext\<TMessage\>\>)**

```csharp
public ConnectHandle ConnectHandler(IConsumePipeConnector consumePipe, MessageHandler<TMessage> handler, IBuildPipeConfigurator<ConsumeContext<TMessage>> configurator)
```

#### Parameters

`consumePipe` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`handler` [MessageHandler\<TMessage\>](../../masstransit-abstractions/masstransit/messagehandler-1)<br/>

`configurator` [IBuildPipeConfigurator\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ibuildpipeconfigurator-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectRequestHandler(IRequestPipeConnector, Guid, MessageHandler\<TMessage\>, IBuildPipeConfigurator\<ConsumeContext\<TMessage\>\>)**

```csharp
public ConnectHandle ConnectRequestHandler(IRequestPipeConnector consumePipe, Guid requestId, MessageHandler<TMessage> handler, IBuildPipeConfigurator<ConsumeContext<TMessage>> configurator)
```

#### Parameters

`consumePipe` [IRequestPipeConnector](../../masstransit-abstractions/masstransit/irequestpipeconnector)<br/>

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`handler` [MessageHandler\<TMessage\>](../../masstransit-abstractions/masstransit/messagehandler-1)<br/>

`configurator` [IBuildPipeConfigurator\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ibuildpipeconfigurator-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

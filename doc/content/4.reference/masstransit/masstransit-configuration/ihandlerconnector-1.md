---

title: IHandlerConnector<T>

---

# IHandlerConnector\<T\>

Namespace: MassTransit.Configuration

Connects a message handler to the ConsumePipe

```csharp
public interface IHandlerConnector<T>
```

#### Type Parameters

`T`<br/>
The message type

## Methods

### **ConnectHandler(IConsumePipeConnector, MessageHandler\<T\>, IBuildPipeConfigurator\<ConsumeContext\<T\>\>)**

Connect a message handler for all messages of type T

```csharp
ConnectHandle ConnectHandler(IConsumePipeConnector consumePipe, MessageHandler<T> handler, IBuildPipeConfigurator<ConsumeContext<T>> configurator)
```

#### Parameters

`consumePipe` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`handler` [MessageHandler\<T\>](../../masstransit-abstractions/masstransit/messagehandler-1)<br/>

`configurator` [IBuildPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ibuildpipeconfigurator-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectRequestHandler(IRequestPipeConnector, Guid, MessageHandler\<T\>, IBuildPipeConfigurator\<ConsumeContext\<T\>\>)**

Connect a message handler for messages with the specified RequestId

```csharp
ConnectHandle ConnectRequestHandler(IRequestPipeConnector consumePipe, Guid requestId, MessageHandler<T> handler, IBuildPipeConfigurator<ConsumeContext<T>> configurator)
```

#### Parameters

`consumePipe` [IRequestPipeConnector](../../masstransit-abstractions/masstransit/irequestpipeconnector)<br/>

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`handler` [MessageHandler\<T\>](../../masstransit-abstractions/masstransit/messagehandler-1)<br/>

`configurator` [IBuildPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ibuildpipeconfigurator-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

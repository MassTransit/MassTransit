---

title: InMemoryOutboxConfigurationExtensions

---

# InMemoryOutboxConfigurationExtensions

Namespace: MassTransit

```csharp
public static class InMemoryOutboxConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryOutboxConfigurationExtensions](../masstransit/inmemoryoutboxconfigurationextensions)

## Methods

### **UseInMemoryOutbox\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>, IRegistrationContext, Action\<IOutboxConfigurator\>)**

Includes an outbox in the consume filter path, which delays outgoing messages until the return path
 of the pipeline returns to the outbox filter. At this point, the message execution pipeline should be
 nearly complete with only the ack remaining. If an exception is thrown, the messages are not sent/published.

```csharp
public static void UseInMemoryOutbox<T>(IPipeConfigurator<ConsumeContext<T>> configurator, IRegistrationContext context, Action<IOutboxConfigurator> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>
The pipe configurator

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`configure` [Action\<IOutboxConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the outbox

### **UseInMemoryOutbox\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>, Action\<IOutboxConfigurator\>)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Includes an outbox in the consume filter path, which delays outgoing messages until the return path
 of the pipeline returns to the outbox filter. At this point, the message execution pipeline should be
 nearly complete with only the ack remaining. If an exception is thrown, the messages are not sent/published.

```csharp
public static void UseInMemoryOutbox<T>(IPipeConfigurator<ConsumeContext<T>> configurator, Action<IOutboxConfigurator> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>
The pipe configurator

`configure` [Action\<IOutboxConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the outbox

### **UseInMemoryOutbox(IConsumePipeConfigurator, IRegistrationContext, Action\<IOutboxConfigurator\>)**

Includes an outbox in the consume filter path, which delays outgoing messages until the return path
 of the pipeline returns to the outbox filter. At this point, the message execution pipeline should be
 nearly complete with only the ack remaining. If an exception is thrown, the messages are not sent/published.

```csharp
public static void UseInMemoryOutbox(IConsumePipeConfigurator configurator, IRegistrationContext context, Action<IOutboxConfigurator> configure)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>
The pipe configurator

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`configure` [Action\<IOutboxConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the outbox

### **UseInMemoryOutbox(IConsumePipeConfigurator, Action\<IOutboxConfigurator\>)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Includes an outbox in the consume filter path, which delays outgoing messages until the return path
 of the pipeline returns to the outbox filter. At this point, the message execution pipeline should be
 nearly complete with only the ack remaining. If an exception is thrown, the messages are not sent/published.

```csharp
public static void UseInMemoryOutbox(IConsumePipeConfigurator configurator, Action<IOutboxConfigurator> configure)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>
The pipe configurator

`configure` [Action\<IOutboxConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the outbox

### **UseInMemoryOutbox\<TConsumer\>(IConsumerConfigurator\<TConsumer\>, IRegistrationContext, Action\<IOutboxConfigurator\>)**

Includes an outbox in the consume filter path, which delays outgoing messages until the return path
 of the pipeline returns to the outbox filter. At this point, the message execution pipeline should be
 nearly complete with only the ack remaining. If an exception is thrown, the messages are not sent/published.

```csharp
public static void UseInMemoryOutbox<TConsumer>(IConsumerConfigurator<TConsumer> configurator, IRegistrationContext context, Action<IOutboxConfigurator> configure)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`configure` [Action\<IOutboxConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the outbox

### **UseInMemoryOutbox\<TConsumer\>(IConsumerConfigurator\<TConsumer\>, Action\<IOutboxConfigurator\>)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Includes an outbox in the consume filter path, which delays outgoing messages until the return path
 of the pipeline returns to the outbox filter. At this point, the message execution pipeline should be
 nearly complete with only the ack remaining. If an exception is thrown, the messages are not sent/published.

```csharp
public static void UseInMemoryOutbox<TConsumer>(IConsumerConfigurator<TConsumer> configurator, Action<IOutboxConfigurator> configure)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

`configure` [Action\<IOutboxConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the outbox

### **UseInMemoryOutbox\<TSaga\>(ISagaConfigurator\<TSaga\>, IRegistrationContext, Action\<IOutboxConfigurator\>)**

Includes an outbox in the consume filter path, which delays outgoing messages until the return path
 of the pipeline returns to the outbox filter. At this point, the message execution pipeline should be
 nearly complete with only the ack remaining. If an exception is thrown, the messages are not sent/published.

```csharp
public static void UseInMemoryOutbox<TSaga>(ISagaConfigurator<TSaga> configurator, IRegistrationContext context, Action<IOutboxConfigurator> configure)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`configure` [Action\<IOutboxConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the outbox

### **UseInMemoryOutbox\<TSaga\>(ISagaConfigurator\<TSaga\>, Action\<IOutboxConfigurator\>)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Includes an outbox in the consume filter path, which delays outgoing messages until the return path
 of the pipeline returns to the outbox filter. At this point, the message execution pipeline should be
 nearly complete with only the ack remaining. If an exception is thrown, the messages are not sent/published.

```csharp
public static void UseInMemoryOutbox<TSaga>(ISagaConfigurator<TSaga> configurator, Action<IOutboxConfigurator> configure)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`configure` [Action\<IOutboxConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the outbox

### **UseInMemoryOutbox\<TMessage\>(IHandlerConfigurator\<TMessage\>, IRegistrationContext, Action\<IOutboxConfigurator\>)**

Includes an outbox in the consume filter path, which delays outgoing messages until the return path
 of the pipeline returns to the outbox filter. At this point, the message execution pipeline should be
 nearly complete with only the ack remaining. If an exception is thrown, the messages are not sent/published.

```csharp
public static void UseInMemoryOutbox<TMessage>(IHandlerConfigurator<TMessage> configurator, IRegistrationContext context, Action<IOutboxConfigurator> configure)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IHandlerConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/ihandlerconfigurator-1)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`configure` [Action\<IOutboxConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the outbox

### **UseInMemoryOutbox\<TMessage\>(IHandlerConfigurator\<TMessage\>, Action\<IOutboxConfigurator\>)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Includes an outbox in the consume filter path, which delays outgoing messages until the return path
 of the pipeline returns to the outbox filter. At this point, the message execution pipeline should be
 nearly complete with only the ack remaining. If an exception is thrown, the messages are not sent/published.

```csharp
public static void UseInMemoryOutbox<TMessage>(IHandlerConfigurator<TMessage> configurator, Action<IOutboxConfigurator> configure)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IHandlerConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/ihandlerconfigurator-1)<br/>

`configure` [Action\<IOutboxConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the outbox

### **AddInMemoryInboxOutbox(IServiceCollection)**

Adds the required components to support the in-memory version of the InboxOutbox, which is intended for
 testing purposes only.

```csharp
public static IServiceCollection AddInMemoryInboxOutbox(IServiceCollection collection)
```

#### Parameters

`collection` IServiceCollection<br/>

#### Returns

IServiceCollection<br/>

### **UseInMemoryInboxOutbox(IReceiveEndpointConfigurator, IRegistrationContext)**

Includes a combination inbox/outbox in the consume pipeline, which stores outgoing messages in memory until
 the message consumer completes.

```csharp
public static void UseInMemoryInboxOutbox(IReceiveEndpointConfigurator configurator, IRegistrationContext context)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
Configuration service provider

### **UseInMemoryInboxOutbox(IReceiveEndpointConfigurator, IServiceProvider)**

#### Caution

Obsolete, use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Includes a combination inbox/outbox in the consume pipeline, which stores outgoing messages in memory until
 the message consumer completes.

```csharp
public static void UseInMemoryInboxOutbox(IReceiveEndpointConfigurator configurator, IServiceProvider provider)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`provider` IServiceProvider<br/>
Configuration service provider

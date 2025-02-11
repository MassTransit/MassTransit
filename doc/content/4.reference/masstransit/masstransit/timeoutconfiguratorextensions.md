---

title: TimeoutConfiguratorExtensions

---

# TimeoutConfiguratorExtensions

Namespace: MassTransit

```csharp
public static class TimeoutConfiguratorExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TimeoutConfiguratorExtensions](../masstransit/timeoutconfiguratorextensions)

## Methods

### **UseTimeout\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>, Action\<ITimeoutConfigurator\>)**

Cancels context's CancellationToken once timeout is reached.

```csharp
public static void UseTimeout<T>(IPipeConfigurator<ConsumeContext<T>> configurator, Action<ITimeoutConfigurator> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>
The pipe configurator

`configure` [Action\<ITimeoutConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure timeout

### **UseTimeout(IConsumePipeConfigurator, Action\<ITimeoutConfigurator\>)**

Cancels context's CancellationToken once timeout is reached.

```csharp
public static void UseTimeout(IConsumePipeConfigurator configurator, Action<ITimeoutConfigurator> configure)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>
The pipe configurator

`configure` [Action\<ITimeoutConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure timeout

### **UseTimeout\<TConsumer\>(IConsumerConfigurator\<TConsumer\>, Action\<ITimeoutConfigurator\>)**

Cancels context's CancellationToken once timeout is reached.

```csharp
public static void UseTimeout<TConsumer>(IConsumerConfigurator<TConsumer> configurator, Action<ITimeoutConfigurator> configure)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>
The pipe configurator

`configure` [Action\<ITimeoutConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure timeout

### **UseTimeout\<TSaga\>(ISagaConfigurator\<TSaga\>, Action\<ITimeoutConfigurator\>)**

Cancels context's CancellationToken once timeout is reached.

```csharp
public static void UseTimeout<TSaga>(ISagaConfigurator<TSaga> configurator, Action<ITimeoutConfigurator> configure)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>
The pipe configurator

`configure` [Action\<ITimeoutConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure timeout

### **UseTimeout\<TMessage\>(IHandlerConfigurator\<TMessage\>, Action\<ITimeoutConfigurator\>)**

Cancels context's CancellationToken once timeout is reached.

```csharp
public static void UseTimeout<TMessage>(IHandlerConfigurator<TMessage> configurator, Action<ITimeoutConfigurator> configure)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IHandlerConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/ihandlerconfigurator-1)<br/>
The pipe configurator

`configure` [Action\<ITimeoutConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure timeout

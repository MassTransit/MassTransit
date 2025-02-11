---

title: DelegatePipeConfiguratorExtensions

---

# DelegatePipeConfiguratorExtensions

Namespace: MassTransit

```csharp
public static class DelegatePipeConfiguratorExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelegatePipeConfiguratorExtensions](../masstransit/delegatepipeconfiguratorextensions)

## Methods

### **UseSendExecute(ISendPipeConfigurator, Action\<SendContext\>)**

Adds a callback filter to the send pipeline

```csharp
public static void UseSendExecute(ISendPipeConfigurator configurator, Action<SendContext> callback)
```

#### Parameters

`configurator` [ISendPipeConfigurator](../../masstransit-abstractions/masstransit/isendpipeconfigurator)<br/>

`callback` [Action\<SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback to invoke

### **UseSendExecuteAsync(ISendPipeConfigurator, Func\<SendContext, Task\>)**

Adds a callback filter to the send pipeline

```csharp
public static void UseSendExecuteAsync(ISendPipeConfigurator configurator, Func<SendContext, Task> callback)
```

#### Parameters

`configurator` [ISendPipeConfigurator](../../masstransit-abstractions/masstransit/isendpipeconfigurator)<br/>

`callback` [Func\<SendContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback to invoke

### **UseSendExecute\<T\>(ISendPipeConfigurator, Action\<SendContext\<T\>\>)**

Adds a callback filter to the send pipeline

```csharp
public static void UseSendExecute<T>(ISendPipeConfigurator configurator, Action<SendContext<T>> callback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [ISendPipeConfigurator](../../masstransit-abstractions/masstransit/isendpipeconfigurator)<br/>

`callback` [Action\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback to invoke

### **UseSendExecuteAsync\<T\>(ISendPipeConfigurator, Func\<SendContext\<T\>, Task\>)**

Adds a callback filter to the send pipeline

```csharp
public static void UseSendExecuteAsync<T>(ISendPipeConfigurator configurator, Func<SendContext<T>, Task> callback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [ISendPipeConfigurator](../../masstransit-abstractions/masstransit/isendpipeconfigurator)<br/>

`callback` [Func\<SendContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback to invoke

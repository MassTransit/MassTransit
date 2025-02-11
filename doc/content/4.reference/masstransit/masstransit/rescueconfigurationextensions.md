---

title: RescueConfigurationExtensions

---

# RescueConfigurationExtensions

Namespace: MassTransit

```csharp
public static class RescueConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RescueConfigurationExtensions](../masstransit/rescueconfigurationextensions)

## Methods

### **UseRescue(IPipeConfigurator\<ReceiveContext\>, IPipe\<ExceptionReceiveContext\>, Action\<IExceptionConfigurator\>)**

Rescue exceptions via the alternate pipe

```csharp
public static void UseRescue(IPipeConfigurator<ReceiveContext> configurator, IPipe<ExceptionReceiveContext> rescuePipe, Action<IExceptionConfigurator> configure)
```

#### Parameters

`configurator` [IPipeConfigurator\<ReceiveContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`rescuePipe` [IPipe\<ExceptionReceiveContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`configure` [Action\<IExceptionConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseRescue(IPipeConfigurator\<ConsumeContext\>, IPipe\<ExceptionConsumeContext\>, Action\<IExceptionConfigurator\>)**

Rescue exceptions via the alternate pipe

```csharp
public static void UseRescue(IPipeConfigurator<ConsumeContext> configurator, IPipe<ExceptionConsumeContext> rescuePipe, Action<IExceptionConfigurator> configure)
```

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`rescuePipe` [IPipe\<ExceptionConsumeContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`configure` [Action\<IExceptionConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseRescue\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>, IPipe\<ExceptionConsumeContext\<T\>\>, Action\<IExceptionConfigurator\>)**

Rescue exceptions via the alternate pipe

```csharp
public static void UseRescue<T>(IPipeConfigurator<ConsumeContext<T>> configurator, IPipe<ExceptionConsumeContext<T>> rescuePipe, Action<IExceptionConfigurator> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`rescuePipe` [IPipe\<ExceptionConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`configure` [Action\<IExceptionConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseRescue\<T\>(IPipeConfigurator\<ConsumerConsumeContext\<T\>\>, IPipe\<ExceptionConsumerConsumeContext\<T\>\>, Action\<IExceptionConfigurator\>)**

Rescue exceptions via the alternate pipe

```csharp
public static void UseRescue<T>(IPipeConfigurator<ConsumerConsumeContext<T>> configurator, IPipe<ExceptionConsumerConsumeContext<T>> rescuePipe, Action<IExceptionConfigurator> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumerConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`rescuePipe` [IPipe\<ExceptionConsumerConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`configure` [Action\<IExceptionConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseRescue\<T\>(IPipeConfigurator\<SagaConsumeContext\<T\>\>, IPipe\<ExceptionSagaConsumeContext\<T\>\>, Action\<IExceptionConfigurator\>)**

Rescue exceptions via the alternate pipe

```csharp
public static void UseRescue<T>(IPipeConfigurator<SagaConsumeContext<T>> configurator, IPipe<ExceptionSagaConsumeContext<T>> rescuePipe, Action<IExceptionConfigurator> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<SagaConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`rescuePipe` [IPipe\<ExceptionSagaConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`configure` [Action\<IExceptionConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseRescue\<TContext, TRescue\>(IPipeConfigurator\<TContext\>, IPipe\<TRescue\>, RescueContextFactory\<TContext, TRescue\>, Action\<IRescueConfigurator\<TContext, TRescue\>\>)**

Rescue exceptions via the alternate pipe

```csharp
public static void UseRescue<TContext, TRescue>(IPipeConfigurator<TContext> configurator, IPipe<TRescue> rescuePipe, RescueContextFactory<TContext, TRescue> rescueContextFactory, Action<IRescueConfigurator<TContext, TRescue>> configure)
```

#### Type Parameters

`TContext`<br/>

`TRescue`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<TContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`rescuePipe` [IPipe\<TRescue\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`rescueContextFactory` [RescueContextFactory\<TContext, TRescue\>](../masstransit-middleware/rescuecontextfactory-2)<br/>
Factory method to convert the pipe context to the rescue pipe context

`configure` [Action\<IRescueConfigurator\<TContext, TRescue\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseRescue\<TContext, TRescue\>(IPipeConfigurator\<TContext\>, RescueContextFactory\<TContext, TRescue\>, Action\<IRescueConfigurator\<TContext, TRescue\>\>)**

Adds a filter to the pipe which is of a different type than the native pipe context type

```csharp
public static void UseRescue<TContext, TRescue>(IPipeConfigurator<TContext> configurator, RescueContextFactory<TContext, TRescue> rescueContextFactory, Action<IRescueConfigurator<TContext, TRescue>> configure)
```

#### Type Parameters

`TContext`<br/>
The context type

`TRescue`<br/>
The filter context type

#### Parameters

`configurator` [IPipeConfigurator\<TContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>
The pipe configurator

`rescueContextFactory` [RescueContextFactory\<TContext, TRescue\>](../masstransit-middleware/rescuecontextfactory-2)<br/>

`configure` [Action\<IRescueConfigurator\<TContext, TRescue\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

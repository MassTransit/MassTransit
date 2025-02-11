---

title: FutureResponseConfigurator<TCommand, TResult, TFault, TRequest, TResponse>

---

# FutureResponseConfigurator\<TCommand, TResult, TFault, TRequest, TResponse\>

Namespace: MassTransit.Configuration

```csharp
public class FutureResponseConfigurator<TCommand, TResult, TFault, TRequest, TResponse> : FutureResponseHandle<TCommand, TResult, TFault, TRequest, TResponse>, FutureRequestHandle<TCommand, TResult, TFault, TRequest>, IFutureResponseConfigurator<TResult, TResponse>, IFutureResultConfigurator<TResult, TResponse>, ISpecification
```

#### Type Parameters

`TCommand`<br/>

`TResult`<br/>

`TFault`<br/>

`TRequest`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureResponseConfigurator\<TCommand, TResult, TFault, TRequest, TResponse\>](../masstransit-configuration/futureresponseconfigurator-5)<br/>
Implements [FutureResponseHandle\<TCommand, TResult, TFault, TRequest, TResponse\>](../masstransit/futureresponsehandle-5), [FutureRequestHandle\<TCommand, TResult, TFault, TRequest\>](../masstransit/futurerequesthandle-4), [IFutureResponseConfigurator\<TResult, TResponse\>](../masstransit/ifutureresponseconfigurator-2), [IFutureResultConfigurator\<TResult, TResponse\>](../masstransit/ifutureresultconfigurator-2), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **PendingResponseIdProvider**

```csharp
public PendingFutureIdProvider<TResponse> PendingResponseIdProvider { get; private set; }
```

#### Property Value

[PendingFutureIdProvider\<TResponse\>](../masstransit/pendingfutureidprovider-1)<br/>

### **Completed**

```csharp
public Event<TResponse> Completed { get; }
```

#### Property Value

[Event\<TResponse\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **Faulted**

```csharp
public Event<Fault<TRequest>> Faulted { get; }
```

#### Property Value

[Event\<Fault\<TRequest\>\>](../../masstransit-abstractions/masstransit/event-1)<br/>

## Constructors

### **FutureResponseConfigurator(IFutureStateMachineConfigurator, FutureRequestHandle\<TCommand, TResult, TFault, TRequest\>)**

```csharp
public FutureResponseConfigurator(IFutureStateMachineConfigurator configurator, FutureRequestHandle<TCommand, TResult, TFault, TRequest> request)
```

#### Parameters

`configurator` [IFutureStateMachineConfigurator](../masstransit-futures/ifuturestatemachineconfigurator)<br/>

`request` [FutureRequestHandle\<TCommand, TResult, TFault, TRequest\>](../masstransit/futurerequesthandle-4)<br/>

## Methods

### **OnResponseReceived\<T\>(Action\<IFutureResponseConfigurator\<TResult, T\>\>)**

```csharp
public FutureResponseHandle<TCommand, TResult, TFault, TRequest, T> OnResponseReceived<T>(Action<IFutureResponseConfigurator<TResult, T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configure` [Action\<IFutureResponseConfigurator\<TResult, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[FutureResponseHandle\<TCommand, TResult, TFault, TRequest, T\>](../masstransit/futureresponsehandle-5)<br/>

### **CompletePendingRequest(PendingFutureIdProvider\<TResponse\>)**

```csharp
public void CompletePendingRequest(PendingFutureIdProvider<TResponse> provider)
```

#### Parameters

`provider` [PendingFutureIdProvider\<TResponse\>](../masstransit/pendingfutureidprovider-1)<br/>

### **WhenReceived(Func\<EventActivityBinder\<FutureState, TResponse\>, EventActivityBinder\<FutureState, TResponse\>\>)**

```csharp
public void WhenReceived(Func<EventActivityBinder<FutureState, TResponse>, EventActivityBinder<FutureState, TResponse>> configure)
```

#### Parameters

`configure` [Func\<EventActivityBinder\<FutureState, TResponse\>, EventActivityBinder\<FutureState, TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **SetCompletedUsingFactory(EventMessageFactory\<FutureState, TResponse, TResult\>)**

```csharp
public void SetCompletedUsingFactory(EventMessageFactory<FutureState, TResponse, TResult> factoryMethod)
```

#### Parameters

`factoryMethod` [EventMessageFactory\<FutureState, TResponse, TResult\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

### **SetCompletedUsingFactory(AsyncEventMessageFactory\<FutureState, TResponse, TResult\>)**

```csharp
public void SetCompletedUsingFactory(AsyncEventMessageFactory<FutureState, TResponse, TResult> factoryMethod)
```

#### Parameters

`factoryMethod` [AsyncEventMessageFactory\<FutureState, TResponse, TResult\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

### **SetCompletedUsingInitializer(InitializerValueProvider\<TResponse\>)**

```csharp
public void SetCompletedUsingInitializer(InitializerValueProvider<TResponse> valueProvider)
```

#### Parameters

`valueProvider` [InitializerValueProvider\<TResponse\>](../masstransit/initializervalueprovider-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **SetResult(BehaviorContext\<FutureState, TResponse\>)**

```csharp
public Task SetResult(BehaviorContext<FutureState, TResponse> context)
```

#### Parameters

`context` [BehaviorContext\<FutureState, TResponse\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

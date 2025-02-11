---

title: FutureRequestConfigurator<TCommand, TResult, TFault, TInput, TRequest>

---

# FutureRequestConfigurator\<TCommand, TResult, TFault, TInput, TRequest\>

Namespace: MassTransit.Configuration

```csharp
public class FutureRequestConfigurator<TCommand, TResult, TFault, TInput, TRequest> : FutureRequestHandle<TCommand, TResult, TFault, TRequest>, IFutureRequestConfigurator<TFault, TInput, TRequest>, ISpecification
```

#### Type Parameters

`TCommand`<br/>

`TResult`<br/>

`TFault`<br/>

`TInput`<br/>

`TRequest`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureRequestConfigurator\<TCommand, TResult, TFault, TInput, TRequest\>](../masstransit-configuration/futurerequestconfigurator-5)<br/>
Implements [FutureRequestHandle\<TCommand, TResult, TFault, TRequest\>](../masstransit/futurerequesthandle-4), [IFutureRequestConfigurator\<TFault, TInput, TRequest\>](../masstransit/ifuturerequestconfigurator-3), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **PendingRequestIdProvider**

```csharp
public PendingFutureIdProvider<TRequest> PendingRequestIdProvider { get; private set; }
```

#### Property Value

[PendingFutureIdProvider\<TRequest\>](../masstransit/pendingfutureidprovider-1)<br/>

### **Faulted**

```csharp
public Event<Fault<TRequest>> Faulted { get; }
```

#### Property Value

[Event\<Fault\<TRequest\>\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **RequestAddress**

```csharp
public Uri RequestAddress { set; }
```

#### Property Value

Uri<br/>

## Constructors

### **FutureRequestConfigurator(IFutureStateMachineConfigurator, Event\<Fault\<TRequest\>\>)**

```csharp
public FutureRequestConfigurator(IFutureStateMachineConfigurator configurator, Event<Fault<TRequest>> faulted)
```

#### Parameters

`configurator` [IFutureStateMachineConfigurator](../masstransit-futures/ifuturestatemachineconfigurator)<br/>

`faulted` [Event\<Fault\<TRequest\>\>](../../masstransit-abstractions/masstransit/event-1)<br/>

## Methods

### **OnResponseReceived\<TResponse\>(Action\<IFutureResponseConfigurator\<TResult, TResponse\>\>)**

```csharp
public FutureResponseHandle<TCommand, TResult, TFault, TRequest, TResponse> OnResponseReceived<TResponse>(Action<IFutureResponseConfigurator<TResult, TResponse>> configure)
```

#### Type Parameters

`TResponse`<br/>

#### Parameters

`configure` [Action\<IFutureResponseConfigurator\<TResult, TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[FutureResponseHandle\<TCommand, TResult, TFault, TRequest, TResponse\>](../masstransit/futureresponsehandle-5)<br/>

### **SetRequestAddressProvider(RequestAddressProvider\<TInput\>)**

```csharp
public void SetRequestAddressProvider(RequestAddressProvider<TInput> provider)
```

#### Parameters

`provider` [RequestAddressProvider\<TInput\>](../masstransit/requestaddressprovider-1)<br/>

### **UsingRequestFactory(EventMessageFactory\<FutureState, TInput, TRequest\>)**

```csharp
public void UsingRequestFactory(EventMessageFactory<FutureState, TInput, TRequest> factoryMethod)
```

#### Parameters

`factoryMethod` [EventMessageFactory\<FutureState, TInput, TRequest\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

### **UsingRequestFactory(AsyncEventMessageFactory\<FutureState, TInput, TRequest\>)**

```csharp
public void UsingRequestFactory(AsyncEventMessageFactory<FutureState, TInput, TRequest> factoryMethod)
```

#### Parameters

`factoryMethod` [AsyncEventMessageFactory\<FutureState, TInput, TRequest\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

### **UsingRequestInitializer(InitializerValueProvider\<TInput\>)**

```csharp
public void UsingRequestInitializer(InitializerValueProvider<TInput> valueProvider)
```

#### Parameters

`valueProvider` [InitializerValueProvider\<TInput\>](../masstransit/initializervalueprovider-1)<br/>

### **TrackPendingRequest(PendingFutureIdProvider\<TRequest\>)**

```csharp
public void TrackPendingRequest(PendingFutureIdProvider<TRequest> provider)
```

#### Parameters

`provider` [PendingFutureIdProvider\<TRequest\>](../masstransit/pendingfutureidprovider-1)<br/>

### **OnRequestFaulted(Action\<IFutureFaultConfigurator\<TFault, Fault\<TRequest\>\>\>)**

```csharp
public void OnRequestFaulted(Action<IFutureFaultConfigurator<TFault, Fault<TRequest>>> configure)
```

#### Parameters

`configure` [Action\<IFutureFaultConfigurator\<TFault, Fault\<TRequest\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **WhenFaulted(Func\<EventActivityBinder\<FutureState, Fault\<TRequest\>\>, EventActivityBinder\<FutureState, Fault\<TRequest\>\>\>)**

```csharp
public void WhenFaulted(Func<EventActivityBinder<FutureState, Fault<TRequest>>, EventActivityBinder<FutureState, Fault<TRequest>>> configure)
```

#### Parameters

`configure` [Func\<EventActivityBinder\<FutureState, Fault\<TRequest\>\>, EventActivityBinder\<FutureState, Fault\<TRequest\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Send(BehaviorContext\<FutureState, TInput\>)**

```csharp
public Task Send(BehaviorContext<FutureState, TInput> context)
```

#### Parameters

`context` [BehaviorContext\<FutureState, TInput\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send(BehaviorContext\<FutureState, TCommand\>, TInput)**

```csharp
public Task Send(BehaviorContext<FutureState, TCommand> context, TInput data)
```

#### Parameters

`context` [BehaviorContext\<FutureState, TCommand\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`data` TInput<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendRange(BehaviorContext\<FutureState, TCommand\>, IEnumerable\<TInput\>)**

```csharp
public Task SendRange(BehaviorContext<FutureState, TCommand> context, IEnumerable<TInput> inputs)
```

#### Parameters

`context` [BehaviorContext\<FutureState, TCommand\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`inputs` [IEnumerable\<TInput\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SetFaulted(BehaviorContext\<FutureState, Fault\<TRequest\>\>)**

```csharp
public Task SetFaulted(BehaviorContext<FutureState, Fault<TRequest>> context)
```

#### Parameters

`context` [BehaviorContext\<FutureState, Fault\<TRequest\>\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

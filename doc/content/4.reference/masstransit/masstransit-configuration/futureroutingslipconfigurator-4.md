---

title: FutureRoutingSlipConfigurator<TCommand, TResult, TFault, TInput>

---

# FutureRoutingSlipConfigurator\<TCommand, TResult, TFault, TInput\>

Namespace: MassTransit.Configuration

```csharp
public class FutureRoutingSlipConfigurator<TCommand, TResult, TFault, TInput> : FutureRoutingSlipHandle, IFutureRoutingSlipConfigurator<TResult, TFault, TInput>, ISpecification
```

#### Type Parameters

`TCommand`<br/>

`TResult`<br/>

`TFault`<br/>

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureRoutingSlipConfigurator\<TCommand, TResult, TFault, TInput\>](../masstransit-configuration/futureroutingslipconfigurator-4)<br/>
Implements [FutureRoutingSlipHandle](../masstransit/futureroutingsliphandle), [IFutureRoutingSlipConfigurator\<TResult, TFault, TInput\>](../masstransit/ifutureroutingslipconfigurator-3), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **CompletedIdProvider**

```csharp
public PendingFutureIdProvider<RoutingSlipCompleted> CompletedIdProvider { get; private set; }
```

#### Property Value

[PendingFutureIdProvider\<RoutingSlipCompleted\>](../masstransit/pendingfutureidprovider-1)<br/>

### **FaultedIdProvider**

```csharp
public PendingFutureIdProvider<RoutingSlipFaulted> FaultedIdProvider { get; private set; }
```

#### Property Value

[PendingFutureIdProvider\<RoutingSlipFaulted\>](../masstransit/pendingfutureidprovider-1)<br/>

### **Completed**

```csharp
public Event<RoutingSlipCompleted> Completed { get; }
```

#### Property Value

[Event\<RoutingSlipCompleted\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **Faulted**

```csharp
public Event<RoutingSlipFaulted> Faulted { get; }
```

#### Property Value

[Event\<RoutingSlipFaulted\>](../../masstransit-abstractions/masstransit/event-1)<br/>

## Constructors

### **FutureRoutingSlipConfigurator(IFutureStateMachineConfigurator, Event\<RoutingSlipCompleted\>, Event\<RoutingSlipFaulted\>)**

```csharp
public FutureRoutingSlipConfigurator(IFutureStateMachineConfigurator configurator, Event<RoutingSlipCompleted> routingSlipCompleted, Event<RoutingSlipFaulted> routingSlipFaulted)
```

#### Parameters

`configurator` [IFutureStateMachineConfigurator](../masstransit-futures/ifuturestatemachineconfigurator)<br/>

`routingSlipCompleted` [Event\<RoutingSlipCompleted\>](../../masstransit-abstractions/masstransit/event-1)<br/>

`routingSlipFaulted` [Event\<RoutingSlipFaulted\>](../../masstransit-abstractions/masstransit/event-1)<br/>

## Methods

### **OnRoutingSlipCompleted(Action\<IFutureResultConfigurator\<TResult, RoutingSlipCompleted\>\>)**

```csharp
public void OnRoutingSlipCompleted(Action<IFutureResultConfigurator<TResult, RoutingSlipCompleted>> configure)
```

#### Parameters

`configure` [Action\<IFutureResultConfigurator\<TResult, RoutingSlipCompleted\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **OnRoutingSlipFaulted(Action\<IFutureFaultConfigurator\<TFault, RoutingSlipFaulted\>\>)**

```csharp
public void OnRoutingSlipFaulted(Action<IFutureFaultConfigurator<TFault, RoutingSlipFaulted>> configure)
```

#### Parameters

`configure` [Action\<IFutureFaultConfigurator\<TFault, RoutingSlipFaulted\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **WhenRoutingSlipCompleted(Func\<EventActivityBinder\<FutureState, RoutingSlipCompleted\>, EventActivityBinder\<FutureState, RoutingSlipCompleted\>\>)**

```csharp
public void WhenRoutingSlipCompleted(Func<EventActivityBinder<FutureState, RoutingSlipCompleted>, EventActivityBinder<FutureState, RoutingSlipCompleted>> configure)
```

#### Parameters

`configure` [Func\<EventActivityBinder\<FutureState, RoutingSlipCompleted\>, EventActivityBinder\<FutureState, RoutingSlipCompleted\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **WhenRoutingSlipFaulted(Func\<EventActivityBinder\<FutureState, RoutingSlipFaulted\>, EventActivityBinder\<FutureState, RoutingSlipFaulted\>\>)**

```csharp
public void WhenRoutingSlipFaulted(Func<EventActivityBinder<FutureState, RoutingSlipFaulted>, EventActivityBinder<FutureState, RoutingSlipFaulted>> configure)
```

#### Parameters

`configure` [Func\<EventActivityBinder\<FutureState, RoutingSlipFaulted\>, EventActivityBinder\<FutureState, RoutingSlipFaulted\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **TrackPendingRoutingSlip()**

```csharp
public void TrackPendingRoutingSlip()
```

### **BuildItinerary(BuildItineraryCallback\<TInput\>)**

```csharp
public void BuildItinerary(BuildItineraryCallback<TInput> buildItinerary)
```

#### Parameters

`buildItinerary` [BuildItineraryCallback\<TInput\>](../masstransit/builditinerarycallback-1)<br/>

### **BuildUsingItineraryPlanner()**

```csharp
public void BuildUsingItineraryPlanner()
```

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **HasResult(FutureResult\<TCommand, TResult, RoutingSlipCompleted\>)**

```csharp
public bool HasResult(out FutureResult<TCommand, TResult, RoutingSlipCompleted> result)
```

#### Parameters

`result` [FutureResult\<TCommand, TResult, RoutingSlipCompleted\>](../masstransit-futures/futureresult-3)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **HasFault(FutureFault\<TCommand, TFault, RoutingSlipFaulted\>)**

```csharp
public bool HasFault(out FutureFault<TCommand, TFault, RoutingSlipFaulted> fault)
```

#### Parameters

`fault` [FutureFault\<TCommand, TFault, RoutingSlipFaulted\>](../masstransit-futures/futurefault-3)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Execute(BehaviorContext\<FutureState, TInput\>)**

```csharp
public Task Execute(BehaviorContext<FutureState, TInput> context)
```

#### Parameters

`context` [BehaviorContext\<FutureState, TInput\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

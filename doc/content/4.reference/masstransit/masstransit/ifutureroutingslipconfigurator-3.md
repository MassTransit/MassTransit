---

title: IFutureRoutingSlipConfigurator<TResult, TFault, TInput>

---

# IFutureRoutingSlipConfigurator\<TResult, TFault, TInput\>

Namespace: MassTransit

```csharp
public interface IFutureRoutingSlipConfigurator<TResult, TFault, TInput>
```

#### Type Parameters

`TResult`<br/>

`TFault`<br/>

`TInput`<br/>

## Methods

### **TrackPendingRoutingSlip()**

If specified, the routing slip is added to the pending results, using the routing slip tracking
 number. When the routing slip completes or faults, the pending result is completed or faulted.

```csharp
void TrackPendingRoutingSlip()
```

### **BuildItinerary(BuildItineraryCallback\<TInput\>)**

Builds the routing slip itinerary when the command is received. The routing slip builder
 is passed, along with the . The tracking numbers,
 subscriptions, and FutureId variables are already initialized.

```csharp
void BuildItinerary(BuildItineraryCallback<TInput> buildItinerary)
```

#### Parameters

`buildItinerary` [BuildItineraryCallback\<TInput\>](../masstransit/builditinerarycallback-1)<br/>

### **BuildUsingItineraryPlanner()**

Builds the routing slip itinerary when the command is received using a container-registered
 [IItineraryPlanner\<TInput\>](../masstransit/iitineraryplanner-1).

```csharp
void BuildUsingItineraryPlanner()
```

### **OnRoutingSlipCompleted(Action\<IFutureResultConfigurator\<TResult, RoutingSlipCompleted\>\>)**

Configure the behavior when the routing slip completes.

```csharp
void OnRoutingSlipCompleted(Action<IFutureResultConfigurator<TResult, RoutingSlipCompleted>> configure)
```

#### Parameters

`configure` [Action\<IFutureResultConfigurator\<TResult, RoutingSlipCompleted\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **OnRoutingSlipFaulted(Action\<IFutureFaultConfigurator\<TFault, RoutingSlipFaulted\>\>)**

Configure what happens when the routing slip faults

```csharp
void OnRoutingSlipFaulted(Action<IFutureFaultConfigurator<TFault, RoutingSlipFaulted>> configure)
```

#### Parameters

`configure` [Action\<IFutureFaultConfigurator\<TFault, RoutingSlipFaulted\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **WhenRoutingSlipCompleted(Func\<EventActivityBinder\<FutureState, RoutingSlipCompleted\>, EventActivityBinder\<FutureState, RoutingSlipCompleted\>\>)**

Add activities to the state machine that are executed when the routing slip is completed

```csharp
void WhenRoutingSlipCompleted(Func<EventActivityBinder<FutureState, RoutingSlipCompleted>, EventActivityBinder<FutureState, RoutingSlipCompleted>> configure)
```

#### Parameters

`configure` [Func\<EventActivityBinder\<FutureState, RoutingSlipCompleted\>, EventActivityBinder\<FutureState, RoutingSlipCompleted\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **WhenRoutingSlipFaulted(Func\<EventActivityBinder\<FutureState, RoutingSlipFaulted\>, EventActivityBinder\<FutureState, RoutingSlipFaulted\>\>)**

Add activities to the state machine that are executed when the routing slip is faulted

```csharp
void WhenRoutingSlipFaulted(Func<EventActivityBinder<FutureState, RoutingSlipFaulted>, EventActivityBinder<FutureState, RoutingSlipFaulted>> configure)
```

#### Parameters

`configure` [Func\<EventActivityBinder\<FutureState, RoutingSlipFaulted\>, EventActivityBinder\<FutureState, RoutingSlipFaulted\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

---

title: IFutureResponseConfigurator<TResult, TResponse>

---

# IFutureResponseConfigurator\<TResult, TResponse\>

Namespace: MassTransit

```csharp
public interface IFutureResponseConfigurator<TResult, TResponse> : IFutureResultConfigurator<TResult, TResponse>
```

#### Type Parameters

`TResult`<br/>

`TResponse`<br/>

Implements [IFutureResultConfigurator\<TResult, TResponse\>](../masstransit/ifutureresultconfigurator-2)

## Methods

### **CompletePendingRequest(PendingFutureIdProvider\<TResponse\>)**

If specified, the identifier is used to complete a pending result and the result will be stored
 in the future.

```csharp
void CompletePendingRequest(PendingFutureIdProvider<TResponse> provider)
```

#### Parameters

`provider` [PendingFutureIdProvider\<TResponse\>](../masstransit/pendingfutureidprovider-1)<br/>
Provides the identifier from the request

### **WhenReceived(Func\<EventActivityBinder\<FutureState, TResponse\>, EventActivityBinder\<FutureState, TResponse\>\>)**

Add activities to the state machine that are executed when the response is received

```csharp
void WhenReceived(Func<EventActivityBinder<FutureState, TResponse>, EventActivityBinder<FutureState, TResponse>> configure)
```

#### Parameters

`configure` [Func\<EventActivityBinder\<FutureState, TResponse\>, EventActivityBinder\<FutureState, TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

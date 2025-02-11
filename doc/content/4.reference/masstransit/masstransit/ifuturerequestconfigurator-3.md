---

title: IFutureRequestConfigurator<TFault, TInput, TRequest>

---

# IFutureRequestConfigurator\<TFault, TInput, TRequest\>

Namespace: MassTransit

```csharp
public interface IFutureRequestConfigurator<TFault, TInput, TRequest>
```

#### Type Parameters

`TFault`<br/>

`TInput`<br/>

`TRequest`<br/>

## Properties

### **RequestAddress**

Set the request destination address. If not specified, the request will be published.

```csharp
public abstract Uri RequestAddress { set; }
```

#### Property Value

Uri<br/>

## Methods

### **SetRequestAddressProvider(RequestAddressProvider\<TInput\>)**

Set the request destination address dynamically using the provider

```csharp
void SetRequestAddressProvider(RequestAddressProvider<TInput> provider)
```

#### Parameters

`provider` [RequestAddressProvider\<TInput\>](../masstransit/requestaddressprovider-1)<br/>

### **UsingRequestFactory(EventMessageFactory\<FutureState, TInput, TRequest\>)**

Create the request using a factory method.

```csharp
void UsingRequestFactory(EventMessageFactory<FutureState, TInput, TRequest> factoryMethod)
```

#### Parameters

`factoryMethod` [EventMessageFactory\<FutureState, TInput, TRequest\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>
Returns the request message

### **UsingRequestFactory(AsyncEventMessageFactory\<FutureState, TInput, TRequest\>)**

Create the request using an asynchronous factory method.

```csharp
void UsingRequestFactory(AsyncEventMessageFactory<FutureState, TInput, TRequest> factoryMethod)
```

#### Parameters

`factoryMethod` [AsyncEventMessageFactory\<FutureState, TInput, TRequest\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>
Returns the request message

### **UsingRequestInitializer(InitializerValueProvider\<TInput\>)**

Create the request using a message initializer. The initiating command is also used to initialize
 request properties prior to apply the values specified.

```csharp
void UsingRequestInitializer(InitializerValueProvider<TInput> valueProvider)
```

#### Parameters

`valueProvider` [InitializerValueProvider\<TInput\>](../masstransit/initializervalueprovider-1)<br/>
Returns an object of values to initialize the request

### **TrackPendingRequest(PendingFutureIdProvider\<TRequest\>)**

If specified, the request is added to the pending results, using the identifier returned by the
 provider. A subsequent result with a matching identifier will complete the pending result.

```csharp
void TrackPendingRequest(PendingFutureIdProvider<TRequest> provider)
```

#### Parameters

`provider` [PendingFutureIdProvider\<TRequest\>](../masstransit/pendingfutureidprovider-1)<br/>
Provides the identifier from the request

### **OnRequestFaulted(Action\<IFutureFaultConfigurator\<TFault, Fault\<TRequest\>\>\>)**

Configure what happens when the request faults

```csharp
void OnRequestFaulted(Action<IFutureFaultConfigurator<TFault, Fault<TRequest>>> configure)
```

#### Parameters

`configure` [Action\<IFutureFaultConfigurator\<TFault, Fault\<TRequest\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **WhenFaulted(Func\<EventActivityBinder\<FutureState, Fault\<TRequest\>\>, EventActivityBinder\<FutureState, Fault\<TRequest\>\>\>)**

Add activities to the state machine that are executed when the request faults

```csharp
void WhenFaulted(Func<EventActivityBinder<FutureState, Fault<TRequest>>, EventActivityBinder<FutureState, Fault<TRequest>>> configure)
```

#### Parameters

`configure` [Func\<EventActivityBinder\<FutureState, Fault\<TRequest\>\>, EventActivityBinder\<FutureState, Fault\<TRequest\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

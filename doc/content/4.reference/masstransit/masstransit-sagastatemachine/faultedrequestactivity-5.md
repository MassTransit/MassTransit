---

title: FaultedRequestActivity<TInstance, TData, TException, TRequest, TResponse>

---

# FaultedRequestActivity\<TInstance, TData, TException, TRequest, TResponse\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class FaultedRequestActivity<TInstance, TData, TException, TRequest, TResponse> : RequestActivityImpl<TInstance, TRequest, TResponse>, IStateMachineActivity<TInstance, TData>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

`TRequest`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestActivityImpl\<TInstance, TRequest, TResponse\>](../masstransit-sagastatemachine/requestactivityimpl-3) → [FaultedRequestActivity\<TInstance, TData, TException, TRequest, TResponse\>](../masstransit-sagastatemachine/faultedrequestactivity-5)<br/>
Implements [IStateMachineActivity\<TInstance, TData\>](../../masstransit-abstractions/masstransit/istatemachineactivity-2), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FaultedRequestActivity(Request\<TInstance, TRequest, TResponse\>, ContextMessageFactory\<BehaviorExceptionContext\<TInstance, TData, TException\>, TRequest\>)**

```csharp
public FaultedRequestActivity(Request<TInstance, TRequest, TResponse> request, ContextMessageFactory<BehaviorExceptionContext<TInstance, TData, TException>, TRequest> messageFactory)
```

#### Parameters

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>

`messageFactory` [ContextMessageFactory\<BehaviorExceptionContext\<TInstance, TData, TException\>, TRequest\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **FaultedRequestActivity(Request\<TInstance, TRequest, TResponse\>, ServiceAddressExceptionProvider\<TInstance, TData, TException\>, ContextMessageFactory\<BehaviorExceptionContext\<TInstance, TData, TException\>, TRequest\>)**

```csharp
public FaultedRequestActivity(Request<TInstance, TRequest, TResponse> request, ServiceAddressExceptionProvider<TInstance, TData, TException> serviceAddressProvider, ContextMessageFactory<BehaviorExceptionContext<TInstance, TData, TException>, TRequest> messageFactory)
```

#### Parameters

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>

`serviceAddressProvider` [ServiceAddressExceptionProvider\<TInstance, TData, TException\>](../../masstransit-abstractions/masstransit/serviceaddressexceptionprovider-3)<br/>

`messageFactory` [ContextMessageFactory\<BehaviorExceptionContext\<TInstance, TData, TException\>, TRequest\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

## Methods

### **Accept(StateMachineVisitor)**

```csharp
public void Accept(StateMachineVisitor visitor)
```

#### Parameters

`visitor` [StateMachineVisitor](../../masstransit-abstractions/masstransit/statemachinevisitor)<br/>

### **Execute(BehaviorContext\<TInstance, TData\>, IBehavior\<TInstance, TData\>)**

```csharp
public Task Execute(BehaviorContext<TInstance, TData> context, IBehavior<TInstance, TData> next)
```

#### Parameters

`context` [BehaviorContext\<TInstance, TData\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`next` [IBehavior\<TInstance, TData\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<T\>(BehaviorExceptionContext\<TInstance, TData, T\>, IBehavior\<TInstance, TData\>)**

```csharp
public Task Faulted<T>(BehaviorExceptionContext<TInstance, TData, T> context, IBehavior<TInstance, TData> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TInstance, TData, T\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-3)<br/>

`next` [IBehavior\<TInstance, TData\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

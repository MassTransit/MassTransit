---

title: RequestActivity<TInstance, TData, TRequest, TResponse>

---

# RequestActivity\<TInstance, TData, TRequest, TResponse\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class RequestActivity<TInstance, TData, TRequest, TResponse> : RequestActivityImpl<TInstance, TRequest, TResponse>, IStateMachineActivity<TInstance, TData>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TRequest`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestActivityImpl\<TInstance, TRequest, TResponse\>](../masstransit-sagastatemachine/requestactivityimpl-3) → [RequestActivity\<TInstance, TData, TRequest, TResponse\>](../masstransit-sagastatemachine/requestactivity-4)<br/>
Implements [IStateMachineActivity\<TInstance, TData\>](../../masstransit-abstractions/masstransit/istatemachineactivity-2), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **RequestActivity(Request\<TInstance, TRequest, TResponse\>, ContextMessageFactory\<BehaviorContext\<TInstance, TData\>, TRequest\>)**

```csharp
public RequestActivity(Request<TInstance, TRequest, TResponse> request, ContextMessageFactory<BehaviorContext<TInstance, TData>, TRequest> messageFactory)
```

#### Parameters

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>

`messageFactory` [ContextMessageFactory\<BehaviorContext\<TInstance, TData\>, TRequest\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **RequestActivity(Request\<TInstance, TRequest, TResponse\>, ServiceAddressProvider\<TInstance, TData\>, ContextMessageFactory\<BehaviorContext\<TInstance, TData\>, TRequest\>)**

```csharp
public RequestActivity(Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance, TData> serviceAddressProvider, ContextMessageFactory<BehaviorContext<TInstance, TData>, TRequest> messageFactory)
```

#### Parameters

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>

`serviceAddressProvider` [ServiceAddressProvider\<TInstance, TData\>](../../masstransit-abstractions/masstransit/serviceaddressprovider-2)<br/>

`messageFactory` [ContextMessageFactory\<BehaviorContext\<TInstance, TData\>, TRequest\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

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

### **Faulted\<TException\>(BehaviorExceptionContext\<TInstance, TData, TException\>, IBehavior\<TInstance, TData\>)**

```csharp
public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, IBehavior<TInstance, TData> next)
```

#### Type Parameters

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TInstance, TData, TException\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-3)<br/>

`next` [IBehavior\<TInstance, TData\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

---

title: RequestActivity<TInstance, TRequest, TResponse>

---

# RequestActivity\<TInstance, TRequest, TResponse\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class RequestActivity<TInstance, TRequest, TResponse> : RequestActivityImpl<TInstance, TRequest, TResponse>, IStateMachineActivity<TInstance>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TInstance`<br/>

`TRequest`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestActivityImpl\<TInstance, TRequest, TResponse\>](../masstransit-sagastatemachine/requestactivityimpl-3) → [RequestActivity\<TInstance, TRequest, TResponse\>](../masstransit-sagastatemachine/requestactivity-3)<br/>
Implements [IStateMachineActivity\<TInstance\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **RequestActivity(Request\<TInstance, TRequest, TResponse\>, ContextMessageFactory\<BehaviorContext\<TInstance\>, TRequest\>)**

```csharp
public RequestActivity(Request<TInstance, TRequest, TResponse> request, ContextMessageFactory<BehaviorContext<TInstance>, TRequest> messageFactory)
```

#### Parameters

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>

`messageFactory` [ContextMessageFactory\<BehaviorContext\<TInstance\>, TRequest\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **RequestActivity(Request\<TInstance, TRequest, TResponse\>, ServiceAddressProvider\<TInstance\>, ContextMessageFactory\<BehaviorContext\<TInstance\>, TRequest\>)**

```csharp
public RequestActivity(Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance> serviceAddressProvider, ContextMessageFactory<BehaviorContext<TInstance>, TRequest> messageFactory)
```

#### Parameters

`request` [Request\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>

`serviceAddressProvider` [ServiceAddressProvider\<TInstance\>](../../masstransit-abstractions/masstransit/serviceaddressprovider-1)<br/>

`messageFactory` [ContextMessageFactory\<BehaviorContext\<TInstance\>, TRequest\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

## Methods

### **Accept(StateMachineVisitor)**

```csharp
public void Accept(StateMachineVisitor visitor)
```

#### Parameters

`visitor` [StateMachineVisitor](../../masstransit-abstractions/masstransit/statemachinevisitor)<br/>

### **Execute(BehaviorContext\<TInstance\>, IBehavior\<TInstance\>)**

```csharp
public Task Execute(BehaviorContext<TInstance> context, IBehavior<TInstance> next)
```

#### Parameters

`context` [BehaviorContext\<TInstance\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`next` [IBehavior\<TInstance\>](../../masstransit-abstractions/masstransit/ibehavior-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Execute\<T\>(BehaviorContext\<TInstance, T\>, IBehavior\<TInstance, T\>)**

```csharp
public Task Execute<T>(BehaviorContext<TInstance, T> context, IBehavior<TInstance, T> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorContext\<TInstance, T\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`next` [IBehavior\<TInstance, T\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<TException\>(BehaviorExceptionContext\<TInstance, TException\>, IBehavior\<TInstance\>)**

```csharp
public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, IBehavior<TInstance> next)
```

#### Type Parameters

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TInstance, TException\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-2)<br/>

`next` [IBehavior\<TInstance\>](../../masstransit-abstractions/masstransit/ibehavior-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<T, TException\>(BehaviorExceptionContext\<TInstance, T, TException\>, IBehavior\<TInstance, T\>)**

```csharp
public Task Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, IBehavior<TInstance, T> next)
```

#### Type Parameters

`T`<br/>

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TInstance, T, TException\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-3)<br/>

`next` [IBehavior\<TInstance, T\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

---

title: FaultedRequestActivity<TSaga, TException, TRequest, TResponse>

---

# FaultedRequestActivity\<TSaga, TException, TRequest, TResponse\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class FaultedRequestActivity<TSaga, TException, TRequest, TResponse> : RequestActivityImpl<TSaga, TRequest, TResponse>, IStateMachineActivity<TSaga>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TRequest`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestActivityImpl\<TSaga, TRequest, TResponse\>](../masstransit-sagastatemachine/requestactivityimpl-3) → [FaultedRequestActivity\<TSaga, TException, TRequest, TResponse\>](../masstransit-sagastatemachine/faultedrequestactivity-4)<br/>
Implements [IStateMachineActivity\<TSaga\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FaultedRequestActivity(Request\<TSaga, TRequest, TResponse\>, ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, TRequest\>)**

```csharp
public FaultedRequestActivity(Request<TSaga, TRequest, TResponse> request, ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, TRequest> messageFactory)
```

#### Parameters

`request` [Request\<TSaga, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>

`messageFactory` [ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, TRequest\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

### **FaultedRequestActivity(Request\<TSaga, TRequest, TResponse\>, ServiceAddressExceptionProvider\<TSaga, TException\>, ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, TRequest\>)**

```csharp
public FaultedRequestActivity(Request<TSaga, TRequest, TResponse> request, ServiceAddressExceptionProvider<TSaga, TException> serviceAddressProvider, ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, TRequest> messageFactory)
```

#### Parameters

`request` [Request\<TSaga, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>

`serviceAddressProvider` [ServiceAddressExceptionProvider\<TSaga, TException\>](../../masstransit-abstractions/masstransit/serviceaddressexceptionprovider-2)<br/>

`messageFactory` [ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, TRequest\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

## Methods

### **Accept(StateMachineVisitor)**

```csharp
public void Accept(StateMachineVisitor visitor)
```

#### Parameters

`visitor` [StateMachineVisitor](../../masstransit-abstractions/masstransit/statemachinevisitor)<br/>

### **Execute(BehaviorContext\<TSaga\>, IBehavior\<TSaga\>)**

```csharp
public Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
```

#### Parameters

`context` [BehaviorContext\<TSaga\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`next` [IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Execute\<T\>(BehaviorContext\<TSaga, T\>, IBehavior\<TSaga, T\>)**

```csharp
public Task Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorContext\<TSaga, T\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`next` [IBehavior\<TSaga, T\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<T\>(BehaviorExceptionContext\<TSaga, T\>, IBehavior\<TSaga\>)**

```csharp
public Task Faulted<T>(BehaviorExceptionContext<TSaga, T> context, IBehavior<TSaga> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, T\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-2)<br/>

`next` [IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

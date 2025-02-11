---

title: SendActivity<TSaga, TData, TMessage>

---

# SendActivity\<TSaga, TData, TMessage\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class SendActivity<TSaga, TData, TMessage> : IStateMachineActivity<TSaga, TData>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendActivity\<TSaga, TData, TMessage\>](../masstransit-sagastatemachine/sendactivity-3)<br/>
Implements [IStateMachineActivity\<TSaga, TData\>](../../masstransit-abstractions/masstransit/istatemachineactivity-2), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **SendActivity(DestinationAddressProvider\<TSaga, TData\>, ContextMessageFactory\<BehaviorContext\<TSaga, TData\>, TMessage\>)**

```csharp
public SendActivity(DestinationAddressProvider<TSaga, TData> destinationAddressProvider, ContextMessageFactory<BehaviorContext<TSaga, TData>, TMessage> messageFactory)
```

#### Parameters

`destinationAddressProvider` [DestinationAddressProvider\<TSaga, TData\>](../../masstransit-abstractions/masstransit/destinationaddressprovider-2)<br/>

`messageFactory` [ContextMessageFactory\<BehaviorContext\<TSaga, TData\>, TMessage\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

## Methods

### **Accept(StateMachineVisitor)**

```csharp
public void Accept(StateMachineVisitor inspector)
```

#### Parameters

`inspector` [StateMachineVisitor](../../masstransit-abstractions/masstransit/statemachinevisitor)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Execute(BehaviorContext\<TSaga, TData\>, IBehavior\<TSaga, TData\>)**

```csharp
public Task Execute(BehaviorContext<TSaga, TData> context, IBehavior<TSaga, TData> next)
```

#### Parameters

`context` [BehaviorContext\<TSaga, TData\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`next` [IBehavior\<TSaga, TData\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<TException\>(BehaviorExceptionContext\<TSaga, TData, TException\>, IBehavior\<TSaga, TData\>)**

```csharp
public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TData, TException> context, IBehavior<TSaga, TData> next)
```

#### Type Parameters

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TData, TException\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-3)<br/>

`next` [IBehavior\<TSaga, TData\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

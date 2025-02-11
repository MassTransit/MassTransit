---

title: IRoutingSlipExecutor<TInput>

---

# IRoutingSlipExecutor\<TInput\>

Namespace: MassTransit.Futures

```csharp
public interface IRoutingSlipExecutor<TInput>
```

#### Type Parameters

`TInput`<br/>

## Properties

### **TrackRoutingSlip**

```csharp
public abstract bool TrackRoutingSlip { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **Execute(BehaviorContext\<FutureState, TInput\>)**

```csharp
Task Execute(BehaviorContext<FutureState, TInput> context)
```

#### Parameters

`context` [BehaviorContext\<FutureState, TInput\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

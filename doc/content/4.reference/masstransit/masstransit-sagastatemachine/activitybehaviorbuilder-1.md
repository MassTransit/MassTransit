---

title: ActivityBehaviorBuilder<TSaga>

---

# ActivityBehaviorBuilder\<TSaga\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class ActivityBehaviorBuilder<TSaga> : IBehaviorBuilder<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ActivityBehaviorBuilder\<TSaga\>](../masstransit-sagastatemachine/activitybehaviorbuilder-1)<br/>
Implements [IBehaviorBuilder\<TSaga\>](../masstransit-sagastatemachine/ibehaviorbuilder-1)

## Properties

### **Behavior**

```csharp
public IBehavior<TSaga> Behavior { get; }
```

#### Property Value

[IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1)<br/>

## Constructors

### **ActivityBehaviorBuilder()**

```csharp
public ActivityBehaviorBuilder()
```

## Methods

### **Add(IStateMachineActivity\<TSaga\>)**

```csharp
public void Add(IStateMachineActivity<TSaga> activity)
```

#### Parameters

`activity` [IStateMachineActivity\<TSaga\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1)<br/>

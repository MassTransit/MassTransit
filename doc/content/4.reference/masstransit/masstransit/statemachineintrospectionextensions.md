---

title: StateMachineIntrospectionExtensions

---

# StateMachineIntrospectionExtensions

Namespace: MassTransit

```csharp
public static class StateMachineIntrospectionExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StateMachineIntrospectionExtensions](../masstransit/statemachineintrospectionextensions)

## Methods

### **NextEvents\<TInstance\>(BehaviorContext\<TInstance\>)**

```csharp
public static Task<IEnumerable<Event>> NextEvents<TInstance>(BehaviorContext<TInstance> context)
```

#### Type Parameters

`TInstance`<br/>

#### Parameters

`context` [BehaviorContext\<TInstance\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

#### Returns

[Task\<IEnumerable\<Event\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

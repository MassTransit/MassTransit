---

title: IStateMachineActivitySelector<TInstance>

---

# IStateMachineActivitySelector\<TInstance\>

Namespace: MassTransit

```csharp
public interface IStateMachineActivitySelector<TInstance>
```

#### Type Parameters

`TInstance`<br/>

## Methods

### **OfType\<TActivity\>()**

An activity which accepts the instance and data from the event

```csharp
EventActivityBinder<TInstance> OfType<TActivity>()
```

#### Type Parameters

`TActivity`<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

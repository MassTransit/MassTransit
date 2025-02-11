---

title: IStateMachineActivitySelector<TInstance, TData>

---

# IStateMachineActivitySelector\<TInstance, TData\>

Namespace: MassTransit

```csharp
public interface IStateMachineActivitySelector<TInstance, TData>
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

## Methods

### **OfType\<TActivity\>()**

An activity which accepts the instance and data from the event

```csharp
EventActivityBinder<TInstance, TData> OfType<TActivity>()
```

#### Type Parameters

`TActivity`<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **OfInstanceType\<TActivity\>()**

An activity that only accepts the instance, and does not require the event data

```csharp
EventActivityBinder<TInstance, TData> OfInstanceType<TActivity>()
```

#### Type Parameters

`TActivity`<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

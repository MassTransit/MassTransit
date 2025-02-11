---

title: IStateMachineFaultedActivitySelector<TInstance, TException>

---

# IStateMachineFaultedActivitySelector\<TInstance, TException\>

Namespace: MassTransit

```csharp
public interface IStateMachineFaultedActivitySelector<TInstance, TException>
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

## Methods

### **OfType\<TActivity\>()**

An activity which accepts the instance and data from the event

```csharp
ExceptionActivityBinder<TInstance, TException> OfType<TActivity>()
```

#### Type Parameters

`TActivity`<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

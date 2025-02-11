---

title: IStateMachineFaultedActivitySelector<TInstance, TData, TException>

---

# IStateMachineFaultedActivitySelector\<TInstance, TData, TException\>

Namespace: MassTransit

```csharp
public interface IStateMachineFaultedActivitySelector<TInstance, TData, TException>
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

## Methods

### **OfType\<TActivity\>()**

An activity which accepts the instance and data from the event

```csharp
ExceptionActivityBinder<TInstance, TData, TException> OfType<TActivity>()
```

#### Type Parameters

`TActivity`<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **OfInstanceType\<TActivity\>()**

An activity that only accepts the instance, and does not require the event data

```csharp
ExceptionActivityBinder<TInstance, TData, TException> OfInstanceType<TActivity>()
```

#### Type Parameters

`TActivity`<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

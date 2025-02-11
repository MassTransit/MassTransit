---

title: ICompositeEventStatusAccessor<TSaga>

---

# ICompositeEventStatusAccessor\<TSaga\>

Namespace: MassTransit.SagaStateMachine

```csharp
public interface ICompositeEventStatusAccessor<TSaga> : IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **Get(TSaga)**

```csharp
CompositeEventStatus Get(TSaga instance)
```

#### Parameters

`instance` TSaga<br/>

#### Returns

[CompositeEventStatus](../../masstransit-abstractions/masstransit/compositeeventstatus)<br/>

### **Set(TSaga, CompositeEventStatus)**

```csharp
void Set(TSaga instance, CompositeEventStatus status)
```

#### Parameters

`instance` TSaga<br/>

`status` [CompositeEventStatus](../../masstransit-abstractions/masstransit/compositeeventstatus)<br/>

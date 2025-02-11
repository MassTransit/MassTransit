---

title: NewOrExistingSagaPolicy<TSaga, TMessage>

---

# NewOrExistingSagaPolicy\<TSaga, TMessage\>

Namespace: MassTransit.Saga

Creates a new or uses an existing saga instance

```csharp
public class NewOrExistingSagaPolicy<TSaga, TMessage> : ISagaPolicy<TSaga, TMessage>
```

#### Type Parameters

`TSaga`<br/>
The saga type

`TMessage`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NewOrExistingSagaPolicy\<TSaga, TMessage\>](../masstransit-saga/neworexistingsagapolicy-2)<br/>
Implements [ISagaPolicy\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/isagapolicy-2)

## Properties

### **IsReadOnly**

```csharp
public bool IsReadOnly { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **NewOrExistingSagaPolicy(ISagaFactory\<TSaga, TMessage\>, Boolean)**

```csharp
public NewOrExistingSagaPolicy(ISagaFactory<TSaga, TMessage> sagaFactory, bool insertOnInitial)
```

#### Parameters

`sagaFactory` [ISagaFactory\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/isagafactory-2)<br/>

`insertOnInitial` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **PreInsertInstance(ConsumeContext\<TMessage\>, TSaga)**

```csharp
public bool PreInsertInstance(ConsumeContext<TMessage> context, out TSaga instance)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`instance` TSaga<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

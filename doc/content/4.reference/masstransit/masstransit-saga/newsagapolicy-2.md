---

title: NewSagaPolicy<TSaga, TMessage>

---

# NewSagaPolicy\<TSaga, TMessage\>

Namespace: MassTransit.Saga

Accepts a message to a saga that does not already exist, throwing an exception if an existing
 saga instance is specified.

```csharp
public class NewSagaPolicy<TSaga, TMessage> : ISagaPolicy<TSaga, TMessage>
```

#### Type Parameters

`TSaga`<br/>
The saga type

`TMessage`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NewSagaPolicy\<TSaga, TMessage\>](../masstransit-saga/newsagapolicy-2)<br/>
Implements [ISagaPolicy\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/isagapolicy-2)

## Properties

### **IsReadOnly**

```csharp
public bool IsReadOnly { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **NewSagaPolicy(ISagaFactory\<TSaga, TMessage\>, Boolean)**

```csharp
public NewSagaPolicy(ISagaFactory<TSaga, TMessage> sagaFactory, bool insertOnInitial)
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

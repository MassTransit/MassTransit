---

title: AnyExistingSagaPolicy<TSaga, TMessage>

---

# AnyExistingSagaPolicy\<TSaga, TMessage\>

Namespace: MassTransit.Saga

Sends the message to any existing saga instances, failing silently if no saga instances are found.

```csharp
public class AnyExistingSagaPolicy<TSaga, TMessage> : ISagaPolicy<TSaga, TMessage>
```

#### Type Parameters

`TSaga`<br/>
The saga type

`TMessage`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AnyExistingSagaPolicy\<TSaga, TMessage\>](../masstransit-saga/anyexistingsagapolicy-2)<br/>
Implements [ISagaPolicy\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/isagapolicy-2)

## Properties

### **IsReadOnly**

```csharp
public bool IsReadOnly { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **AnyExistingSagaPolicy(IPipe\<ConsumeContext\<TMessage\>\>, Boolean)**

```csharp
public AnyExistingSagaPolicy(IPipe<ConsumeContext<TMessage>> missingPipe, bool readOnly)
```

#### Parameters

`missingPipe` [IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`readOnly` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

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

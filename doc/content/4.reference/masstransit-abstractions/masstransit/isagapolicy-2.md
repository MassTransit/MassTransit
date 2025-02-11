---

title: ISagaPolicy<TSaga, TMessage>

---

# ISagaPolicy\<TSaga, TMessage\>

Namespace: MassTransit

```csharp
public interface ISagaPolicy<TSaga, TMessage>
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

## Properties

### **IsReadOnly**

If true, changes should not be saved to the saga repository

```csharp
public abstract bool IsReadOnly { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **PreInsertInstance(ConsumeContext\<TMessage\>, TSaga)**

If true, the instance returned should be used to try and insert as a new saga instance, ignoring any failures

```csharp
bool PreInsertInstance(ConsumeContext<TMessage> context, out TSaga instance)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../masstransit/consumecontext-1)<br/>

`instance` TSaga<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
True if the instance should be inserted before invoking the message logic

### **Existing(SagaConsumeContext\<TSaga, TMessage\>, IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>)**

The method invoked when an existing saga instance is present

```csharp
Task Existing(SagaConsumeContext<TSaga, TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
```

#### Parameters

`context` [SagaConsumeContext\<TSaga, TMessage\>](../masstransit/sagaconsumecontext-2)<br/>

`next` [IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Missing(ConsumeContext\<TMessage\>, IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>)**

Invoked when there is not an existing saga instance available

```csharp
Task Missing(ConsumeContext<TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../masstransit/consumecontext-1)<br/>

`next` [IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

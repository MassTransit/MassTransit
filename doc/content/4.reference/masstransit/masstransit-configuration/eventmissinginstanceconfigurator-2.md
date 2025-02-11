---

title: EventMissingInstanceConfigurator<TSaga, TMessage>

---

# EventMissingInstanceConfigurator\<TSaga, TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class EventMissingInstanceConfigurator<TSaga, TMessage> : IMissingInstanceConfigurator<TSaga, TMessage>
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EventMissingInstanceConfigurator\<TSaga, TMessage\>](../masstransit-configuration/eventmissinginstanceconfigurator-2)<br/>
Implements [IMissingInstanceConfigurator\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/imissinginstanceconfigurator-2)

## Constructors

### **EventMissingInstanceConfigurator()**

```csharp
public EventMissingInstanceConfigurator()
```

## Methods

### **Discard()**

```csharp
public IPipe<ConsumeContext<TMessage>> Discard()
```

#### Returns

[IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

### **Fault()**

```csharp
public IPipe<ConsumeContext<TMessage>> Fault()
```

#### Returns

[IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

### **ExecuteAsync(Func\<ConsumeContext\<TMessage\>, Task\>)**

```csharp
public IPipe<ConsumeContext<TMessage>> ExecuteAsync(Func<ConsumeContext<TMessage>, Task> callback)
```

#### Parameters

`callback` [Func\<ConsumeContext\<TMessage\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

### **Execute(Action\<ConsumeContext\<TMessage\>\>)**

```csharp
public IPipe<ConsumeContext<TMessage>> Execute(Action<ConsumeContext<TMessage>> callback)
```

#### Parameters

`callback` [Action\<ConsumeContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

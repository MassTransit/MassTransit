---

title: IMissingInstanceConfigurator<TSaga, TMessage>

---

# IMissingInstanceConfigurator\<TSaga, TMessage\>

Namespace: MassTransit

```csharp
public interface IMissingInstanceConfigurator<TSaga, TMessage>
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

## Methods

### **Discard()**

Discard the event, silently ignoring the missing instance for the event

```csharp
IPipe<ConsumeContext<TMessage>> Discard()
```

#### Returns

[IPipe\<ConsumeContext\<TMessage\>\>](../masstransit/ipipe-1)<br/>

### **Fault()**

Fault the saga consumer, which moves the message to the error queue

```csharp
IPipe<ConsumeContext<TMessage>> Fault()
```

#### Returns

[IPipe\<ConsumeContext\<TMessage\>\>](../masstransit/ipipe-1)<br/>

### **ExecuteAsync(Func\<ConsumeContext\<TMessage\>, Task\>)**

Execute an asynchronous method when the instance is missed, allowing a custom behavior to be specified.

```csharp
IPipe<ConsumeContext<TMessage>> ExecuteAsync(Func<ConsumeContext<TMessage>, Task> callback)
```

#### Parameters

`callback` [Func\<ConsumeContext\<TMessage\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IPipe\<ConsumeContext\<TMessage\>\>](../masstransit/ipipe-1)<br/>

### **Execute(Action\<ConsumeContext\<TMessage\>\>)**

Execute a method when the instance is missed, allowing a custom behavior to be specified.

```csharp
IPipe<ConsumeContext<TMessage>> Execute(Action<ConsumeContext<TMessage>> callback)
```

#### Parameters

`callback` [Action\<ConsumeContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IPipe\<ConsumeContext\<TMessage\>\>](../masstransit/ipipe-1)<br/>

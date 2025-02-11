---

title: Fault<T>

---

# Fault\<T\>

Namespace: MassTransit

A faulted message, published when a message consumer fails to process the message

```csharp
public interface Fault<T> : Fault
```

#### Type Parameters

`T`<br/>

Implements [Fault](../masstransit/fault)

## Properties

### **Message**

The message that faulted

```csharp
public abstract T Message { get; }
```

#### Property Value

T<br/>

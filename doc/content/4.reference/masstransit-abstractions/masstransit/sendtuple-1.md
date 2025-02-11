---

title: SendTuple<T>

---

# SendTuple\<T\>

Namespace: MassTransit

Combines a message and a pipe which can be used to send/publish the message

```csharp
public struct SendTuple<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [SendTuple\<T\>](../masstransit/sendtuple-1)

## Fields

### **Message**

```csharp
public T Message;
```

### **Pipe**

```csharp
public IPipe<SendContext<T>> Pipe;
```

## Constructors

### **SendTuple(T, IPipe\<SendContext\<T\>\>)**

```csharp
public SendTuple(T message, IPipe<SendContext<T>> pipe)
```

#### Parameters

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

### **SendTuple(T)**

```csharp
public SendTuple(T message)
```

#### Parameters

`message` T<br/>

## Methods

### **Deconstruct(T, IPipe\<SendContext\<T\>\>)**

```csharp
public void Deconstruct(out T message, out IPipe<SendContext<T>> pipe)
```

#### Parameters

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

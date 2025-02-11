---

title: MessageEvent<TMessage>

---

# MessageEvent\<TMessage\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class MessageEvent<TMessage> : TriggerEvent, Event, IVisitable, IProbeSite, IComparable<Event>, Event<TMessage>, IEquatable<MessageEvent<TMessage>>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TriggerEvent](../masstransit-sagastatemachine/triggerevent) → [MessageEvent\<TMessage\>](../masstransit-sagastatemachine/messageevent-1)<br/>
Implements [Event](../../masstransit-abstractions/masstransit/event), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IComparable\<Event\>](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable-1), [Event\<TMessage\>](../../masstransit-abstractions/masstransit/event-1), [IEquatable\<MessageEvent\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Fields

### **Instance**

```csharp
public static Event<TMessage> Instance;
```

## Properties

### **Name**

```csharp
public string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **MessageEvent(String)**

```csharp
public MessageEvent(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Accept(StateMachineVisitor)**

```csharp
public void Accept(StateMachineVisitor visitor)
```

#### Parameters

`visitor` [StateMachineVisitor](../../masstransit-abstractions/masstransit/statemachinevisitor)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Equals(MessageEvent\<TMessage\>)**

```csharp
public bool Equals(MessageEvent<TMessage> other)
```

#### Parameters

`other` [MessageEvent\<TMessage\>](../masstransit-sagastatemachine/messageevent-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
